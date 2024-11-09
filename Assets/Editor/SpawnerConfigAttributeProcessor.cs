using System;
using System.Collections.Generic;
using System.Reflection;
using Bones.Gameplay.Waves;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

namespace Game.Editor
{
	[UsedImplicitly]
	public class SpawnerConfigAttributeProcessor : OdinAttributeProcessor<SpawnerConfig>
	{
		public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
		{
			switch (member.Name)
			{
				case "_prefab":
					{
						//attributes.Add<HideLabelAttribute>();
						break;
					}
				case "_dropConfig":
					{
						//attributes.Add<HideLabelAttribute>();
						attributes.Add(new LabelTextAttribute("Drop"));
						attributes.Add(new InlineEditorAttribute());
						break;
					}
				case "_statsConfig":
					{
						//attributes.Add<HideLabelAttribute>();
						attributes.Add(new LabelTextAttribute("Stats"));
						attributes.Add(new InlineEditorAttribute());
						break;
					}
				case "_sharedConfig":
					{
						attributes.Add(new InlineEditorAttribute(InlineEditorObjectFieldModes.Boxed));
						attributes.Add(new InlineButtonAttribute("CreateSharedConfig", "Create")
						{
							ShowIf = "@_sharedConfig == null"
						});
						
						break;
					}
				case "_spawningDelay":
					{
						//attributes.Add<HideLabelAttribute>();
						attributes.Add<InlinePropertyAttribute>();
						//attributes.Add(new FoldoutGroupAttribute("Spawning"));
						attributes.Add(new ValidateInputAttribute("Validate_spawningDelay", "Delay must be positive", InfoMessageType.Warning));
						
						break;
					}
				case "_isRepeating":
					{
						//attributes.Add(new FoldoutGroupAttribute("Spawning"));
						
						break;
					}
				case "_limit":
					{
						//attributes.Add(new FoldoutGroupAttribute("Spawning"));
						attributes.Add(new ShowIfAttribute("_isRepeating"));
						
						break;
					}
				case "_origin":
					{
						//attributes.Add(new FoldoutGroupAttribute("Position", expanded: true));
						attributes.Add(new HideIfAttribute("_sharedConfig"));
						
						break;
					}
				case "_amountResolver":
					{
						//attributes.Add<HideLabelAttribute>();
						attributes.Add(new LabelTextAttribute("Amount"));
						attributes.Add(new BoxGroupAttribute("Amount", false));
						attributes.Add<InlinePropertyAttribute>();
						//attributes.Add(new FoldoutGroupAttribute("Spawning", expanded: true));
						attributes.Add(new ValidateInputAttribute("Validate_amountResolver", "Amount resolver must be selected"));
						
						break;
					}
				case "_positionResolver":
					{
						//attributes.Add<HideLabelAttribute>();
						attributes.Add(new LabelTextAttribute("Position"));
						attributes.Add(new BoxGroupAttribute("Position", false));
						attributes.Add(new HideIfAttribute("_sharedConfig"));
						attributes.Add<InlinePropertyAttribute>();
						//attributes.Add(new FoldoutGroupAttribute("Position"));
						attributes.Add(new ValidateInputAttribute("Validate_positionResolver"));
						
						break;
					}
				case "_enemySizeOffsetResolver":
					{
						//attributes.Add<HideLabelAttribute>();
						attributes.Add(new LabelTextAttribute("Enemy Size Offset"));
						attributes.Add(new BoxGroupAttribute("Enemy Size Offset", false));
						attributes.Add<InlinePropertyAttribute>();
						//attributes.Add(new FoldoutGroupAttribute("ScaleOffset"));
						attributes.Add(new ValidateInputAttribute("Validate_enemyOffsetResolver"));
						
						break;
					}
				
				case "_despawningDistance":
					{
						attributes.Add(new HideIfAttribute("_sharedConfig"));
						
						break;
					}
			}
		}
	}
}