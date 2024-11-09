using System.Collections.Generic;
using System.Threading.Tasks;
using Bones.Gameplay.LoadOperations;

namespace Bones.Gameplay.LevelLoader
{
	public class LevelLoader : ILevelLoader
	{
		private readonly ILoadOperationExecutor _loadOperationExecutor;
		public List<LevelData> levels { get; set; }

		public LevelLoader(ILoadOperationExecutor loadOperationExecutor, List<LevelData> levels)
		{
			_loadOperationExecutor = loadOperationExecutor;
			this.levels = levels;
		}

		public async Task Load(int level)
		{
			await _loadOperationExecutor.Execute(new List<ILoadOperation>
				{
					new TestWaitOperation(),
					new LoadSceneOperation(levels[level].SceneAsset)
				}
				);
		}
	}
}