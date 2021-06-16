using System.Threading.Tasks;

namespace JimmyDore.Services.DialogAlert
{
    public interface IJimmyDoreDialogService
    {
        Task DisplayAlertWithOk(string title, string message, bool withSound = true);
        Task DisplayAlert(string title, string message, string cancelButtonText, bool withSound = true);
        Task<bool> UserAcceptedDisplayAlert(string title, string message, string acceptButtonText, string cancelButtonText, bool withSound = true);
    }
}
