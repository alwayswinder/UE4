#pragma once
#include "ToolsCommon.h"
#include <fbxsdk.h>


namespace primal::tools
{
	struct scene_data;
	struct scene;
	struct mesh;
	struct geometry_import_settings;

	class fbx_context
	{
	public:
		fbx_context(const char* file,scene* scene, scene_data* data)
			:_scene{ scene }, _scene_data{ data }
		{
			assert(file && _scene && _scene_data);
			if (initialize_fbx())
			{
				load_fbx_file(file);
				assert(is_valid());
			}
		}
		~fbx_context()
		{
			_fbx_scene->Destroy();
			_fbx_manager->Destroy();
			ZeroMemory(this, sizeof(fbx_context));
		}
		void get_scene(FbxNode* root = nullptr);
		constexpr bool is_valid() const { return _fbx_manager && _fbx_scene; }
		constexpr f32 scene_scale() const { return _scene_scale; }

	private:
		bool initialize_fbx();
		void load_fbx_file(const char* file);
		void get_mesh(FbxNode* node, utl::vector<mesh>& meshes);
		void get_lod_group(FbxNode* node);

		scene*				_scene{ nullptr };
		scene_data*			_scene_data{ nullptr };
		FbxManager*			_fbx_manager{ nullptr };
		FbxScene*			_fbx_scene{ nullptr };
		f32					_scene_scale{ 1.0f };
	};
}