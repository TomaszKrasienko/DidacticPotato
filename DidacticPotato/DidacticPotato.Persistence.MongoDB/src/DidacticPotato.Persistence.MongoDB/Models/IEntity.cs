using System.Runtime.Serialization;

namespace DidacticPotato.Persistence.MongoDB.Models;

public interface IEntity<T>  
{
    T Id { set; }
}