namespace Player._Infrastructure {
    internal interface IInitializable<TParameters> {
        void Initialize(TParameters parameters);
    }
}
