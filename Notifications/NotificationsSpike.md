# Notifications Spike

One of the things I've looked into is how we could use GOV.UK Notify safetly in an event driven way given "at least once" message delivery. i.e. can we make the message idempotent so that if we process the message a second time we don't send duplicate emails.

This is possible, **within the rentention period only**, using the [optional reference argument](https://docs.notifications.service.gov.uk/net.html#send-an-email-arguments-reference-optional) 

The GOV.UK Notify API also [returns a response that includes an id](https://docs.notifications.service.gov.uk/net.html#send-an-email-response) that we don't currently use.

For the spike I've introduced a [Notifications table](../database/NHSD.GPITBuyingCatalogue.Database/Notifications/Tables/Notifications.sql).
We use the generated `Id` as the reference for the request and capture the response id in `ReceiptId`.

The upcoming notifications feature and stories reference emails for 
- [contract expiry](https://buyingcatalog.visualstudio.com/Buying%20Catalogue/_workitems/edit/23275)
- password expiry

However I think it would make sense for all emails to go through the same mechanism so I've also looked at the emails that are sent when the order is completed.
Since these are existing emails with GOV.UK Notify templates and dependencies it was a good way to factor in these complications.

So [OrderService.CompleteOrder](../src/NHSD.GPIT.BuyingCatalogue.Services/Orders/OrderService.cs)
no longer calls the GOV.UK Notify API.
Instead we insert a row into the `Notifications` table for each email we want to send with all the information (`To`, `NotificationType` etc..) needed to make the calls at a later date.
All this is part of the same `CompleteOrder` transaction.

I think the transactional behaviour here is important.
It means the intent to send a notification can commit or rollback as part of the transaction so we can avoid data inconsistencies.
I've found a few articles that refer to this as the [outbox pattern](#outbox-pattern).

Each email has it's own [notification type](../src/NHSD.GPIT.BuyingCatalogue.EntityFramework/Notifications/Models/NotificationTypeEnum.cs)
as they use a different template or set of templates.
- BuyerOrderCompleted - [The email to the buyer.](../src/NHSD.GPIT.BuyingCatalogue.EntityFramework/Notifications/Models/BuyerOrderCompletedEmailContent.cs)
- FinanceOrderCompleted - The email to admin/finance.


## Outbox pattern

- https://medium.com/@outfunnel/emitting-domain-events-with-outbox-pattern-dda4691b234a
- https://www.kamilgrzybek.com/blog/posts/the-outbox-pattern

Both these refer to a separate worker/process that is used to actually dispatch the messages to the messaging system.
> *I've not looked to tackle this in the spike. Instead I'm just dispatching the messages after the transaction completes.*

We could still have an actual email fail to send but this would be fairly trivial to spot as we can look for any **older** notification rows that don't have a `ReceiptId` value.

## NotificationFunction

I've added a new Azure Function [NotificationFunction](../src/BuyingCatalogueFunction/Notifications/NotificationFunction.cs) 
that makes the actual GOV.UK Notify call. For each notification it
- Checks if the notification already has a `ReceiptId`, if so we ignore it.
- Checks the status of notification `Id` reference
  - if GOV.UK Notify **is not** aware of it then we'll make the call to the send email
  - if GOV.UK Notify **is** aware of it then we won't make the call to send the email
- Update the notification `ReceiptId` with the response id from GOV.UK Notify


## Contract Expiry

Unlike order completed, this isn't an existing "event" in the system, it's something we need to detect **once** for a given set of business rules and act accordingly.
i.e. We "raise" the event when we detect it and record the fact that it's happened to suppress detecting it again. 

> *I've not attempted to do this in a Domain Driven Design way where the goal would be an logic rich/behaviour rich domain model. That would be quite a change from our existing model which is mostly logic and behaviour in the services that surround it*

The spike is using an Azure Function [OrderEnteredFirstExpiryThresholdFunction](../src/BuyingCatalogueFunction/Notifications/OrderEnteredFirstExpiryThresholdFunction.cs) 
with a timer trigger to periodically detect orders approaching the contract end date. In this case event `OrderEnteredFirstExpiryThreshold`.
As part of the `OrderEnteredFirstExpiryThreshold` transaction, for each order we:

- Save `OrderEnteredFirstExpiryThreshold` to the OrderEvents table.
- Save the notifications for each user in a similar manner to order completed.

## Azure.Storage.Queues
The recommendation for
[cross function communication](https://learn.microsoft.com/en-us/azure/azure-functions/performance-reliability#cross-function-communication)
is to use storage queues and I think we don't currently have any requirements that would push us to look at alternatives.
We don't need guaranteed FIFO ordered delivery and we don't need publish/subscribe - [point to point](https://www.enterpriseintegrationpatterns.com/patterns/messaging/PointToPointChannel.html#:~:text=Send%20the%20message%20on%20a,successfully%20consume%20a%20particular%20message.)
communication is sufficient.

So notifications from `OrderEnteredFirstExpiryThresholdFunction` are dispatched to `NotificationFunction` via a storage queue called `notifications`.
The `NotifictionFunction` uses a [queue trigger](https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-queue-trigger?tabs=python-v2%2Cisolated-process%2Cnodejs-v4%2Cextensionv5&pivots=programming-language-csharp)
to act as a [polling consumer](https://www.enterpriseintegrationpatterns.com/patterns/messaging/PollingConsumer.html)
to the queue.

For our test environments this queue is created as part of docker azurite seed process similar to the blob storage container.


Complete order dispatches messages to the same queue. So both the function app and services projects have taken a reference to `Azure.Storage.Queues`

## Email Preferences

The [EventTypes](../src/NHSD.GPIT.BuyingCatalogue.EntityFramework/Notifications/Models/EventTypeEnum.cs)
used in the spike are finer grained than the email preferences we want to manage
i.e. we want to manage [email preferences for "Contract Expiry"](https://buyingcatalog.visualstudio.com/Buying%20Catalogue/_workitems/edit/23347)
but this is made up of two distinct events. One for the first email `OrderEnteredFirstExpiryThreshold` and one for the reminder `OrderEnteredSecondExpiryThreshold`.

This is why I introduced the [ManagedEmailPreferences](../database/NHSD.GPITBuyingCatalogue.Database/Notifications/Tables/ManagedEmailPreferences.sql) table.
It has one row for each email preference we want to manage.
[Events](../database/NHSD.GPITBuyingCatalogue.Database/Notifications/Tables/Events.sql)
map to the email preference they belong to, so that `OrderEnteredFirstExpiryThreshold` and  `OrderEnteredSecondExpiryThreshold` would come under `ContractExpiry`.

`ManagedEmailPreferences` also has a `DefaultEnabled` boolean value.
This is to allow for the configuration of either opt in (`DefaultEnabled` `False`) or opt out (`DefaultEnabled` `True`).

I also created a table [UserManagedEmailPreferences](../database/NHSD.GPITBuyingCatalogue.Database/Notifications/Tables/UserManagedEmailPreferences.sql)
to store the user's preference where it differs from `DefaultEnabled`.

> *I've not implemented the code to interrogate `UserManagedEmailPreferences` as part of this spike.*

