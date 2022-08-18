namespace Mvp.Selections.Domain
{
    public class SystemRole : Role
    {
        public SystemRole(Guid id)
            : base(id)
        {
        }

        public Right Rights { get; set; }
    }
}
