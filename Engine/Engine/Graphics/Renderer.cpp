#include "Renderer.h"
#include "GraphicsPlatformInterface.h"
#include "Direct3D12/D3D12Interface.h"

namespace primal::graphics
{
	namespace
	{
		platform_interface gfx{};

		bool set_platform_interface(graphics_platform platform)
		{
			switch (platform)
			{
			case graphics_platform::direct3d12:
				d3d12::get_platform_interface(gfx);
				break;
			default:
				break;
			}
			return false;
		}
	}
	bool initialize(graphics_platform platform)
	{
		return set_platform_interface(platform);
	}

	void shutdown()
	{
		gfx.shutdown();
	}
}
