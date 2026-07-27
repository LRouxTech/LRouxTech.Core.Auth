using System.Linq.Expressions;
using LRouxTech.Core.BaseModel;
using Microsoft.EntityFrameworkCore;

namespace Temp;

public static class ModelBuilderExtensions
{
    public static ModelBuilder ApplyArchivedQueryFilter(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            if (IsSubclassOfRawGeneric(typeof(BaseModel<>), clrType))
            {
                var parameter = Expression.Parameter(clrType, "e");

                var property = Expression.Property(parameter, "ArchivedOn");
                var nullConstant = Expression.Constant(null, typeof(DateTime?));
                var comparison = Expression.Equal(property, nullConstant);
                var lambda = Expression.Lambda(comparison, parameter);

                modelBuilder.Entity(clrType).HasQueryFilter(lambda);
            }
        }

        return modelBuilder;
    }

    private static bool IsSubclassOfRawGeneric(Type genericBase, Type? toCheck)
    {
        while (toCheck != null && toCheck != typeof(object))
        {
            var current = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (genericBase == current)
            {
                return true;
            }
            toCheck = toCheck.BaseType;
        }
        return false;
    }
}