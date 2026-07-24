using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using BuyingCatalogueFunction.Notifications;
using BuyingCatalogueFunction.Notifications.InactiveAccount;
using BuyingCatalogueFunction.Notifications.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NSubstitute.Core;
using Xunit;

namespace BuyingCatalogueFunctionTests.Notifications.InactiveUsers;

public static class InactiveAccountsFunctionTests
{
    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task Run_NoInactiveUsers(
        [Frozen] ILogger<InactiveAccountsFunction> logger,
        [Frozen] IInactiveAccountsService inactiveAccountsService,
        InactiveAccountsFunction inactiveAccountsFunction)
    {
        SetLoggingLevels(logger);

        var timerInfo = new TimerInfo
        {
            ScheduleStatus = new ScheduleStatus
            {
                Last = DateTime.UtcNow.AddDays(-1),
                Next = DateTime.UtcNow.AddDays(1),
                LastUpdated = DateTime.UtcNow,
            },
        };

        inactiveAccountsService.GetInactiveAccounts(Arg.Any<DateOnly>()).Returns([]);

        await inactiveAccountsFunction.Run(timerInfo);

        var logs = GetLogMessages(logger);

        logs.Should().Contain(log =>
            log.LogLevel == LogLevel.Information.ToString() &&
            log.Message.StartsWith("Inactive Accounts: Executed at"));

        logs.Should().Contain(log =>
            log.LogLevel == LogLevel.Information.ToString() &&
            log.Message.StartsWith("Inactive Accounts: Next timer schedule at"));

        logs.Should().Contain(log =>
            log.LogLevel == LogLevel.Information.ToString() &&
            log.Message.Equals("Inactive Accounts: Evaluating Inactive Users"));

        logs.Should().Contain(log =>
            log.LogLevel == LogLevel.Information.ToString() &&
            log.Message.Equals("Inactive Accounts: No inactive users found"));
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task Run_WithInactiveUsers(
        [Frozen] ILogger<InactiveAccountsFunction> logger,
        [Frozen] IInactiveAccountsService inactiveAccountsService,
        [Frozen] IEmailPreferenceService emailPreferenceService,
        InactiveAccountsFunction inactiveAccountsFunction)
    {
        SetLoggingLevels(logger);

        var timerInfo = new TimerInfo
        {
            ScheduleStatus = new ScheduleStatus
            {
                Last = DateTime.UtcNow.AddDays(-1),
                Next = DateTime.UtcNow.AddDays(1),
                LastUpdated = DateTime.UtcNow,
            },
        };

        var users = new List<AspNetUser>
        {
            new() { Id = 1 },
            new() { Id = 2 },
        };

        inactiveAccountsService.GetInactiveAccounts(Arg.Any<DateOnly>())
            .Returns(users);
        emailPreferenceService.GetDefaultEmailPreference(EmailPreferenceTypeEnum.InactiveAccount)
            .Returns(new EmailPreferenceType());
        emailPreferenceService.ShouldTriggerForUser(Arg.Any<EmailPreferenceType>(), Arg.Any<int>())
            .Returns(true);

        await inactiveAccountsFunction.Run(timerInfo);

        var logs = GetLogMessages(logger);

        logs.Should().Contain(log =>
            log.LogLevel == LogLevel.Information.ToString() &&
            log.Message.StartsWith("Inactive Accounts: Executed at"));

        logs.Should().Contain(log =>
            log.LogLevel == LogLevel.Information.ToString() &&
            log.Message.StartsWith("Inactive Accounts: Next timer schedule at"));

        logs.Should().Contain(log =>
            log.LogLevel == LogLevel.Information.ToString() &&
            log.Message.Equals("Inactive Accounts: Evaluating Inactive Users"));

        await inactiveAccountsService
            .Received(1)
            .Raise(
                Arg.Is<AspNetUser>(x => x.Id == users[0].Id),
                Arg.Any<DateOnly>(),
                Arg.Any<EmailPreferenceType>(),
                true);

        await inactiveAccountsService
            .Received(1)
            .Raise(
                Arg.Is<AspNetUser>(x => x.Id == users[1].Id),
                Arg.Any<DateOnly>(),
                Arg.Any<EmailPreferenceType>(),
                true);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task Run_WithUsersUnsubscribedToNotifications(
        [Frozen] ILogger<InactiveAccountsFunction> logger,
        [Frozen] IInactiveAccountsService inactiveAccountsService,
        [Frozen] IEmailPreferenceService emailPreferenceService,
        InactiveAccountsFunction inactiveAccountsFunction)
    {
        SetLoggingLevels(logger);

        var timerInfo = new TimerInfo
        {
            ScheduleStatus = new ScheduleStatus
            {
                Last = DateTime.UtcNow.AddDays(-1),
                Next = DateTime.UtcNow.AddDays(1),
                LastUpdated = DateTime.UtcNow,
            },
        };

        var users = new List<AspNetUser>
        {
            new() { Id = 1 }
        };

        inactiveAccountsService.GetInactiveAccounts(Arg.Any<DateOnly>())
            .Returns(users);
        emailPreferenceService.GetDefaultEmailPreference(EmailPreferenceTypeEnum.InactiveAccount)
            .Returns(new EmailPreferenceType());
        emailPreferenceService.ShouldTriggerForUser(Arg.Any<EmailPreferenceType>(), users[0].Id)
            .Returns(false);

        await inactiveAccountsFunction.Run(timerInfo);

    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task Run_CatchesException_HandlesAsExpected(
        [Frozen] ILogger<InactiveAccountsFunction> logger,
        [Frozen] IInactiveAccountsService inactiveAccountsService,
        [Frozen] IEmailPreferenceService emailPreferenceService,
        InactiveAccountsFunction inactiveAccountsFunction)
    {
        SetLoggingLevels(logger);

        var timerInfo = new TimerInfo
        {
            ScheduleStatus = new ScheduleStatus
            {
                Last = DateTime.UtcNow.AddDays(-1),
                Next = DateTime.UtcNow.AddDays(1),
                LastUpdated = DateTime.UtcNow,
            },
        };

        var users = new List<AspNetUser>
        {
            new() { Id = 1 },
        };

        inactiveAccountsService.GetInactiveAccounts(Arg.Any<DateOnly>())
            .Returns(users);
        inactiveAccountsService.Raise(Arg.Any<AspNetUser>(), Arg.Any<DateOnly>(), Arg.Any<EmailPreferenceType>(), true)
            .Returns(Task.FromException(new InvalidOperationException("Failed to dispatch notification")));
        emailPreferenceService.GetDefaultEmailPreference(EmailPreferenceTypeEnum.InactiveAccount)
            .Returns(new EmailPreferenceType());
        emailPreferenceService.ShouldTriggerForUser(Arg.Any<EmailPreferenceType>(), Arg.Any<int>())
            .Returns(true);

        await inactiveAccountsFunction.Run(timerInfo);

        var logs = GetLogMessages(logger);

        logs.Should().Contain(log =>
            log.LogLevel == LogLevel.Information.ToString() &&
            log.Message.StartsWith("Inactive Accounts: Executed at"));

        logs.Should().Contain(log =>
            log.LogLevel == LogLevel.Information.ToString() &&
            log.Message.StartsWith("Inactive Accounts: Next timer schedule at"));

        logs.Should().Contain(log =>
            log.LogLevel == LogLevel.Information.ToString() &&
            log.Message.Equals("Inactive Accounts: Evaluating Inactive Users"));

        logs.Should().Contain(log =>
            log.LogLevel == LogLevel.Error.ToString() &&
            log.Message.StartsWith("Inactive Accounts: Exception raising a deactivation notice for User"));
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task Run_NullDefaultEmailPreference_HandlesAsExpected(
        [Frozen] ILogger<InactiveAccountsFunction> logger,
        [Frozen] IInactiveAccountsService inactiveAccountsService,
        InactiveAccountsFunction inactiveAccountsFunction)
    {
        SetLoggingLevels(logger);

        var timerInfo = new TimerInfo
        {
            ScheduleStatus = new ScheduleStatus
            {
                Last = DateTime.UtcNow.AddDays(-1),
                Next = DateTime.UtcNow.AddDays(1),
                LastUpdated = DateTime.UtcNow,
            },
        };

        var users = new List<AspNetUser>
        {
            new() { Id = 1 },
        };

        inactiveAccountsService.GetInactiveAccounts(Arg.Any<DateOnly>())
            .Returns(users);
        inactiveAccountsService.Raise(Arg.Any<AspNetUser>(), Arg.Any<DateOnly>(), Arg.Any<EmailPreferenceType>(), true)
            .Returns(Task.FromException(new InvalidOperationException("Failed to dispatch notification")));

        await inactiveAccountsFunction.Run(timerInfo);

        var logs = GetLogMessages(logger);

        logs.Should().Contain(log =>
            log.LogLevel == LogLevel.Information.ToString() &&
            log.Message.StartsWith("Inactive Accounts: Executed at"));

        logs.Should().Contain(log =>
            log.LogLevel == LogLevel.Information.ToString() &&
            log.Message.StartsWith("Inactive Accounts: Next timer schedule at"));

        logs.Should().Contain(log =>
            log.LogLevel == LogLevel.Information.ToString() &&
            log.Message.Equals("Inactive Accounts: Evaluating Inactive Users"));

        logs.Should().Contain(log =>
            log.LogLevel == LogLevel.Error.ToString() &&
            log.Message.Equals("Inactive Accounts: InactiveAccount not found or a ManagedEmailPreference is not configured"));
    }

    private static void SetLoggingLevels(ILogger logger)
    {
        logger.IsEnabled(LogLevel.Information).Returns(true);
        logger.IsEnabled(LogLevel.Warning).Returns(true);
        logger.IsEnabled(LogLevel.Error).Returns(true);
    }

    static bool IsExpectedLogCall(ICall call, LogLevel level, string expectedMessage)
    {
        var arguments = call.GetArguments();

        if (arguments.Length == 0)
        {
            return false;
        }

        return call.GetMethodInfo().Name == nameof(ILogger.Log)
            && arguments.Length > 2
            && arguments[0] is LogLevel actualLogLevel
            && actualLogLevel == level
            && arguments[2]?.ToString()?.Contains(expectedMessage) == true;
    }

    private static List<(string LogLevel, string Message)> GetLogMessages(ILogger logger)
    {
        return [.. logger.ReceivedCalls()
            .Where(call => call.GetMethodInfo().Name == nameof(ILogger.Log))
            .Select(call =>
            {
                var arguments = call.GetArguments();

                return (
                    LogLevel: arguments[0]?.ToString() ?? string.Empty,
                    Message: arguments[2]?.ToString() ?? string.Empty);
            })];
    }
}
