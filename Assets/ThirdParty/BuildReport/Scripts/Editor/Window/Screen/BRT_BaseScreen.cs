using UnityEngine;

namespace BuildReportTool.Window.Screen
{
	public abstract class BaseScreen
	{
		public abstract string Name { get; }

		public abstract void RefreshData(BuildInfo buildReport, AssetDependencies assetDependencies,
			TextureData textureData, MeshData meshData, PrefabData prefabData, UnityBuildReport unityBuildReport, BuildReportTool.AssetBundleSession assetBundleSession);

		public abstract void DrawGUI(Rect position,
			BuildInfo buildReportToDisplay, AssetDependencies assetDependencies,
			TextureData textureData, MeshData meshData, PrefabData prefabData,
			UnityBuildReport unityBuildReport, BuildReportTool.ExtraData extraData, BuildReportTool.AssetBundleSession assetBundleSession,
			out bool requestRepaint);

		public virtual void Update(double timeNow, double deltaTime, BuildInfo buildReportToDisplay, BuildReportTool.AssetBundleSession assetBundleSession,
			AssetDependencies assetDependencies)
		{
		}


		protected void DrawCentralMessage(Rect position, string msg)
		{
			float w = 300;
			float h = 100;
			float x = (position.width - w) * 0.5f;
			float y = (position.height - h) * 0.35f;

			GUI.Label(new Rect(x, y, w, h), msg);
		}
	}
}