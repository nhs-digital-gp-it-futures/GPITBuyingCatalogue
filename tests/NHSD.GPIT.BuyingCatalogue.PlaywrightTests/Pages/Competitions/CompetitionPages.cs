using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Competitions.PriceAndQuantity;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Competitions.Setup;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Competitions.Steps;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Login;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepTwo;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Competitions;

public class CompetitionPages
{
    private readonly ITestOutputHelper _output;
    private readonly CompetitionTestData _data;

    public LoginPage Login { get; }
    public CompetitionsDashboardPage Dashboard { get; }
    public BeforeYouStartPage BeforeYouStart { get; }
    public SelectShortlistPage SelectShortlist { get; }
    public CreateCompetitionPage CreateCompetition { get; }
    public RefineShortlistPage RefineShortlist { get; }
    public ExcludedSolutionsPage ExcludedSolutions { get; }
    public ConfirmShortlistPage ConfirmShortlist { get; }
    public CompetitionTaskListPage TaskList { get; }
    public ServiceRecipientsPage ServiceRecipients { get; }
    public ContractLengthPage ContractLength { get; }
    public AwardCriteriaPage AwardCriteria { get; }
    public CalculatePricePage CalculatePrice { get; }
    public PriceAndQuantityPage PriceAndQuantity { get; }
    public ViewResultsPage ViewResults { get; }

    public CompetitionPages(IPage page, ITestOutputHelper output, CompetitionTestData data)
    {
        _output = output;
        _data = data;

        Login = new LoginPage(page);
        Dashboard = new CompetitionsDashboardPage(page);
        BeforeYouStart = new BeforeYouStartPage(page);
        SelectShortlist = new SelectShortlistPage(page);
        CreateCompetition = new CreateCompetitionPage(page);
        RefineShortlist = new RefineShortlistPage(page);
        ExcludedSolutions = new ExcludedSolutionsPage(page);
        ConfirmShortlist = new ConfirmShortlistPage(page);
        TaskList = new CompetitionTaskListPage(page);
        ServiceRecipients = new ServiceRecipientsPage(page);
        ContractLength = new ContractLengthPage(page);
        AwardCriteria = new AwardCriteriaPage(page);
        CalculatePrice = new CalculatePricePage(page);
        PriceAndQuantity = new PriceAndQuantityPage(page);
        ViewResults = new ViewResultsPage(page);
    }

    public async Task LoginAsync()
    {
        _output.WriteLine("Login");
        await Login.NavigateAsync(_data.BaseUrl);
        await Login.LoginAsync(_data.Email, _data.Password);
    }

    public async Task PrepareCompetitionAsync()
    {
        _output.WriteLine($"Create competition: {_data.CompetitionName}");
        await Dashboard.GoToCompetitionsAsync();
        await Dashboard.StartNewCompetitionAsync();
        await BeforeYouStart.ContinueAsync();
        await SelectShortlist.SelectShortlistAndContinueAsync(_data.ShortlistValue);
        await CreateCompetition.StartCreateAsync();
        await CreateCompetition.EnterNameAndContinueAsync(_data.CompetitionName, _data.CompetitionDescription);

        await RefineShortlist.AssertOnPageAsync();
        await RefineShortlist.SelectSolutionsAndContinueAsync(_data.FirstSolution, _data.SecondSolution);

        await ExcludedSolutions.EnterReasonAndContinueAsync(_data.ExclusionReason);
        await ConfirmShortlist.ConfirmAsync();
        await TaskList.AssertOnPageAsync();
    }

    public async Task CompleteStepOneAsync()
    {
        _output.WriteLine("Step 1: service recipients and contract length");

        await TaskList.GoToServiceRecipientsAsync();
        await ServiceRecipients.AssertOnPageAsync();
        await ServiceRecipients.SelectRecipientsManuallyAsync(
            _data.Sublocation, _data.Practices, serviceCategory: "competition");
        await TaskList.AssertOnPageAsync();
        
        await TaskList.GoToContractLengthAsync();
        await ContractLength.EnterLengthAndContinueAsync(_data.ContractLength);
        await TaskList.AssertOnPageAsync();
    }

    public async Task DefineCompetitionCriteriaAsync()
    {
        _output.WriteLine($"Step 2: award criteria ({_data.AwardCriteria})");

        await TaskList.GoToAwardCriteriaAsync();
        await AwardCriteria.SelectCriteriaAndContinueAsync(_data.AwardCriteria);
        await TaskList.AssertOnPageAsync();
    }

    public async Task CompareAndScoreSolutionsAsync()
    {
        _output.WriteLine("Step 3: calculate price for all solutions and services");

        await TaskList.GoToCalculatePriceAsync();
        await CalculatePrice.AssertOnPageAsync();
        await CalculatePrice.StartFirstSolutionAsync();
        await PriceAndQuantity.AssertOnPageAsync();

        // Emis catalogue solution
        await PriceAndQuantity.SetPriceAsync(CompetitionServiceType.CatalogueSolution);
        await PriceAndQuantity.AssertOnPageAsync();
        await PriceAndQuantity.SetQuantityAsync(
            CompetitionServiceType.CatalogueSolution, _data.BankfieldTotal, _data.BeechwoodTotal);
        await PriceAndQuantity.AssertOnPageAsync();

        // Emis additional service
        await PriceAndQuantity.SetPriceAsync(CompetitionServiceType.AdditionalService);
        await PriceAndQuantity.SetQuantityAsync(
            CompetitionServiceType.AdditionalService, _data.BankfieldTotal, _data.BeechwoodTotal);
        await PriceAndQuantity.SaveAndContinueLinkAsync();

        // Write on Time catalogue solution
        await CalculatePrice.StartSolutionAsync(_data.SecondSolution);
        await PriceAndQuantity.AssertOnPageAsync();
        await PriceAndQuantity.SetPriceAsync(CompetitionServiceType.CatalogueSolution, _data.PriceOption);
        await PriceAndQuantity.AssertOnPageAsync();
        await PriceAndQuantity.SetQuantityAsync(
            CompetitionServiceType.CatalogueSolution, _data.BankfieldTotal, _data.BeechwoodTotal);
        await PriceAndQuantity.AssertOnPageAsync();

        // Write on Time additional service
        await PriceAndQuantity.SetPriceAsync(CompetitionServiceType.AdditionalService);
        await PriceAndQuantity.SetQuantityAsync(
            CompetitionServiceType.AdditionalService, _data.BankfieldTotal, _data.BeechwoodTotal);
        await PriceAndQuantity.SaveAndContinueLinkAsync();

        await CalculatePrice.FinishAsync();
        await TaskList.AssertOnPageAsync();
    }

    public async Task FinishAndViewResultsAsync()
    {
        _output.WriteLine("Step 4: finish and view results");

        await TaskList.GoToViewResultsAsync();
        await ViewResults.ConfirmAndViewResultsAsync();
        await ViewResults.AssertResultsShownAsync();
    }
}
