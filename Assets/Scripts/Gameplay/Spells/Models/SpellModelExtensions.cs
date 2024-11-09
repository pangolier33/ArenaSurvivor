using System;
using UniRx;
using UnityEngine;

namespace Bones.Gameplay.Spells.Classes
{
	public static class SpellModelExtensions
	{
		private static ISpellModel s_emptyInstance;
		public static ISpellModel SingletonEmpty => s_emptyInstance ??= new PlaceholderSpellModel();

		public static ISpellModel Empty() => new PlaceholderSpellModel();
		public static IReadOnlyReactiveProperty<int> LevelProperty(this ISpellModel model) => Cast<LeveledSpellBranch>(model)?.Level;
		public static int LevelValue(this ISpellModel model)
		{
			if (TryCast(model, out LeveledSpellBranch leveledSpellBranch))
				return leveledSpellBranch.Level.Value;
			if (TryCast(model, out DisposableSpellBranch disposableSpellBranch))
				return disposableSpellBranch.IsActive ? 1 : 0;
			return 0;
		}

		public static int MaxLevel(this ISpellModel model)
		{
			if (TryCast(model, out LeveledSpellBranch leveledSpellBranch))
				return leveledSpellBranch.MaxLevel;
			if (TryCast(model, out DisposableSpellBranch _))
				return 1;
			return 0;
		}

		public static bool IsEmpty(this ISpellModel model) => Cast<PlaceholderSpellModel>(model) != null;
		
		public static ClassName Class(this ISpellModel model) => Cast<ClassContainer.SpellModelWrapper>(model)?.Class ?? ClassName.Neutral;
		public static ClassContainer.SpellType Type(this ISpellModel model)
		{
			if (IsEmpty(model))
				return ClassContainer.SpellType.Empty;
			return Cast<ClassContainer.SpellModelWrapper>(model)?.Type ?? ClassContainer.SpellType.Buff;
		}

		private static T Cast<T>(ISpellModel source) where T : class, ISpellModel =>
			source switch
			{
				T model => model,
				ISpellModelWrapper wrapper => Cast<T>(wrapper.Wrapped),
				_ => null,
			};

		private static bool TryCast<T>(ISpellModel source, out T result) where T : class, ISpellModel
		{
			result = Cast<T>(source);
			return result != null;
		}
		
		private class PlaceholderSpellModel : ISpellModel
		{
			public string Name => string.Empty;
			public string Description => string.Empty;
			public Sprite Icon => null;
			public float Weight => 0;
			public ISpellBranch OutBranch => null;
			public IDisposable Apply() => Disposable.Empty;
		}
	}
}