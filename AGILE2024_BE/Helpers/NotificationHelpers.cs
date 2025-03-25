using AGILE2024_BE.Models;

internal static class NotificationHelpers
{

    public static string GetNotificationTitle(EnumNotificationType notificationType)
    {
        switch (notificationType)
        {
            case EnumNotificationType.GoalCreatedNotificationType:
                return "Nový cieľ priradený!";
            case EnumNotificationType.GoalUpdatedNotificationType:
                return "Cieľ aktualizovaný!";
            case EnumNotificationType.FeedbackUnsentReminderNotificationType:
                return "Neodoslaná spätná väzba";
            case EnumNotificationType.ReviewUnsentReminderNotificationType:
                return "Neodoslaný posudok cieľa";
            case EnumNotificationType.GoalUnfinishedReminderNotificationType:
                return "Nedokončený cieľ!";
            case EnumNotificationType.NewSuccessionNotificationType:
                return "Nové nástupníctvo!";
            default:
                return "Nová notifikácia!";
        }
    }
}