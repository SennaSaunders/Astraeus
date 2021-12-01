namespace Code.ObserverPattern {
    public interface ISubject {
        public void AddObserver(IIntObserver intObserver);
        public void RemoveObserver(IIntObserver intObserver);
        public void NotifyObservers();
    }
}