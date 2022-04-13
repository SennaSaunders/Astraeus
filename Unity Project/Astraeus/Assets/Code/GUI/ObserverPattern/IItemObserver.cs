namespace Code.GUI.ObserverPattern {
    public interface IItemObserver<T> {
        public void UpdateSelf(T value);
        public bool UpdateNeeded(T value);
    }
}