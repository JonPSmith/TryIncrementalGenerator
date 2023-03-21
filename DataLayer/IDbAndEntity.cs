using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    //This is the marker for the source generator to create a database query 
    public interface IDbAndEntity<TContext, TEntity> 
        where TContext : DbContext where TEntity : class 
    {

    }
}