using Thanh_Video_Labeler.DataContext;
using Thanh_Video_Labeler.Models;
using Thanh_Video_Labeler.Repositories.Common;

namespace Thanh_Video_Labeler.Repositories.SheetRepository
{
    public interface ISheetCommandRepository : ICommandRepository<Sheet>
    {
    }

    public class SheetCommandRepository(AppDbContext context) : CommandRepository<Sheet>(context), ISheetCommandRepository
    {
    }
}
