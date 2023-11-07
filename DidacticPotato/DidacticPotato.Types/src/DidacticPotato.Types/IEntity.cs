namespace DidacticPotato.Types;

public interface IEntity<T>
{
    T Id { get; set; }
}