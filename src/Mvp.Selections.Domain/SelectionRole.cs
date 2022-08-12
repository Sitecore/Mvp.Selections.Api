namespace Mvp.Selections.Domain
{
    public class SelectionRole : Role
    {
        public Application? Application { get; set; }

        public Country? Country { get; set; }

        public MvpType? MvpType { get; set; }

        public Region? Region { get; set; }

        public Selection? Selection { get; set; }
    }
}
