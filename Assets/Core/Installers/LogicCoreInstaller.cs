using UnityEngine;
using Zenject;

public class LogicCoreInstaller : MonoInstaller
{
    [SerializeField]
    private TetrisGrid _grid;

    [SerializeField]
    private Spawner Spawner;

    public override void InstallBindings()
    {
        Container.Bind<TetrisGrid>().FromInstance(_grid);

        Container.Bind<Spawner>().FromInstance(Spawner);
    }
}