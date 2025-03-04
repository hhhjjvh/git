using System.Threading.Tasks;
using UnityEngine;

public interface IResourceFactory
{
    GameObject GetEnemy(EnemyName name);
    GameObject GetUI(string name);
    GameObject GetFX(string name);
    GameObject GetControllers(string name);
   

}
