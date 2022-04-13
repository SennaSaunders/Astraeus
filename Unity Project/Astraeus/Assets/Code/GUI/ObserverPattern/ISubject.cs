namespace Code.GUI.ObserverPattern {
    public interface ISubject<T> {
        public void AddObserver(T intObserver);
        public void RemoveObserver(T intObserver);
        public void NotifyObservers();
    }
}