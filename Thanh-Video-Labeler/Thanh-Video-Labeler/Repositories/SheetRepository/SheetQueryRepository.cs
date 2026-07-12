using Thanh_Video_Labeler.DataContext;
using Thanh_Video_Labeler.Models;
using Thanh_Video_Labeler.Repositories.Common;

namespace Thanh_Video_Labeler.Repositories.SheetRepository
{
    public interface ISheetQueryRepository : IQueryRepository<Sheet>
    {
    }

    public class SheetQueryRepository(AppDbContext context)
        : QueryRepository<Sheet>(context), ISheetQueryRepository
    {
    }
}
