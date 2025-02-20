using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class LogicCoreInstaller : MonoInstaller
{
    [Header("TetrisGrid")]
    [SerializeField]
    private Transform _rootSpawn;

    public UnityEvent<string> OnRadiusX;

    public UnityEvent<string> OnRadiusY;

    public UnityEvent<string> OnRadiusZ;

    [Header("Spawner")]
    [SerializeField]
    private PresetColors _presetColors;

    [SerializeField]
    private MovementFigure[] _figures;

    public override void InstallBindings()
    {
        BindSpawner(BindTetrisGrid());
    }

    private TetrisGrid BindTetrisGrid()
    {
        TetrisGrid grid = new TetrisGrid(_rootSpawn);

        grid.OnRadiusX += OnRadiusX.Invoke;

        grid.OnRadiusY += OnRadiusY.Invoke;

        grid.OnRadiusZ += OnRadiusZ.Invoke;

        grid.OnRadiusX?.Invoke(grid.Radius.x.ToString());

        grid.OnRadiusY?.Invoke(grid.Radius.y.ToString());

        grid.OnRadiusZ?.Invoke(grid.Radius.z.ToString());

        Container.BindInterfacesAndSelfTo<TetrisGrid>().FromInstance(grid).AsSingle();

        return grid;
    }

    private void BindSpawner(TetrisGrid grid)
    {
        Spawner spawner = new Spawner(_rootSpawn, _presetColors, _figures, grid);

        Container.BindInterfacesAndSelfTo<Spawner>().FromInstance(spawner).AsSingle();
    }
}