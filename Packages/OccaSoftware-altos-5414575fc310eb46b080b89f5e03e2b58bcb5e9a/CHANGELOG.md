# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [7.13.0] - 2024-03-04

### Changed

- You will now control Sky Object Falloff using animation curves. Existing Falloff will be discarded. Caution upgrading.
- You will now control Star Brightness using animation curves. Existing Brightness will be discarded. Caution upgrading.

## [7.12.0] - 2024-02-02

### Added

- Altos now supports a modular lightning system, replacing the old lightning control system.

## [7.11.0] - 2024-01-25

### Added

- Added Color support to Volumetric Clouds for Point Lights (like with the Lightning Array).

## [7.10.0] - 2024-01-23

### Added

- Added a Sky Color Blend property to the Sky Definition. This property lets you configure the rate at which the Sky blends into the Horizon color.

## [7.9.0] - 2024-01-18

### Changed

- Cloud map now uses standalone WeatherMap component and renders using modern Unity `CommandBuffer` and `blitter` APIs.
- Improved editor workflows.
  
## [7.8.3] - 2024-01-17

### Fixed

- Use single-camera origin for Weathermap
  
## [7.8.2] - 2024-01-17

### Fixed

- Removed more redundant logging
- Stars component moved to Altos component menu

## [7.8.1] - 2024-01-11

### Fixed

- Removed redundant logging

## [7.8.0] - 2024-01-11

### Added

- Before, Altos could only render one Star definition. Now, Altos can render as many Star definitions as you want. The Star definition moved out of the Sky Director into the Stars component.
- Sky Objects now support a new property, Tilt.

## [7.7.2] - 2024-01-10

### Fixed

- Split the Cloud Shadow Pass into two passes: one pass to render the Cloud Shadowmap Texture, a second pass to render the Cloud Shadowmap to screen.

## [7.7.1] - 2023-12-04

### Fixed

- Prefixed map and remap functions in math.hlsl; these were colliding with Unity function names.

## [7.7.0] - 2023-12-04

### Changed

- Follow Target is now Precipitation Occlusion Renderer.
- Altos now includes an example Universal Renderer showcasing how to use the Precipitation Occlusion Renderer with the rest of the renderer stack.
- Improved cloud rendering.

## [7.6.1] - 2023-12-04

- Various minor updates and chores.

## [7.6.0] - 2023-12-04

### Added

- Added precipitation occlusion system.
- Added more quality options.
- Added more cloud rendering options.

### Changed

- Updated reprojection handling.
- Updated cloud rendering algorithm.
- Sky Objects used to have a color temperature range and a power value that mapped to that range. This is now controlled with an animation curve.
- Sky Objects used to have an intensity range and a power value that mapped to that range. This is now controlled with an animation curve.

## [7.5.1] - 2023-11-10

### Fixed

- Fixed editor-only code in AltosWindZone.cs that was breaking builds.

## [7.5.0] - 2023-11-08

### Changed

- Cleaned up some prefab scale and position defaults.

## [7.5.0-beta.9] - 2023-11-06

### Added

- Added a minimum wind speed option in the WindZone inspector

### Changed

- Slight changes to the wind speed and direction methods.

### Fixed

- Fixed an issue causing the cloud detail offset to replace the cloud base offset.

## [7.5.0-beta.8] - 2023-11-06

### Fixed

- Fixed an issue when creating an AltosSkyDirector in an unsaved Scene file.

### Added

- Added an Overrides section to the Inspector.

## [7.5.0-beta.7] - 2023-11-06

### Fixed

- Fixed an issue with cloud wind interaction.
- Fixed AltosSkyDirector context menu creation flow.

## [7.5.0-beta.6] - 2023-11-01

### Fixed

- Fixed an issue with the Aurora Borealis shader.

## [7.5.0-beta.5] - 2023-10-31

### Fixed

- Fixed an issue with floating point offset support.

### Changed

- Changed the water and terrain shaders to use the floating point helper functions.

### Added

- Added helper functions for floating point world space position.

## [7.5.0-beta.4] - 2023-10-30

### Fixed

- Fixed a null reference exception when opening the Altos overlay in a scene without Altos.
- Fixed an issue with the AltosCloudsBlend function.

### Changed

- The wind speed now uses gradient noise.

### Added

- Added the time forecast to the forecast debug view.

## [7.5.0-beta.3] - 2023-10-12

### Fixed

-Fixed some remapping issues that broke the sun and precipitation vfx.

### Added

- Added options to control the maximum diurnal temperature variation and maximum temperature anomaly.

## [7.5.0-beta.2] - 2023-10-12

### Added

- Added a toggle to enable distant coverage.
- The Cloud Definition now uses the atmospheric visibility by default. You can override this option with a new toggle in the Cloud Definition, `Override Atmospheric Visibility`.
- Added a system that will automatically set precipitation % over time.
- Added a system that will automatically set cloudiness % over time.

### Changed

- Cloud Definition distances used to be measured in kilometers. They will now be measured in meters. (You may need to manually scale your current values from kilometers to meters).
- Changed the interpolation approach for temperature over time. It now picks a low and high for each day, then interpolates between the two with the low at 6am and the high at 6pm.
- Changed the interpolation approach for wind direction over time. It now gradually changes and won't suddenly reverse direction.
- Wind Direction is now reported using cardinal directions.

### Fixed

- Fixed an issue where the wind direction was never negative.
- Fixed an issue with the cloud weathermap texture option.

### Removed

- Removed various unused properties, including "TAA Enabled," "Cloud Density by Height," "Cloud Detail Height Mapping."

## [7.5.0-beta.1] - 2023-10-11

### Added

- Added a system that will automatically set wind speed and direction over time.
- Added a system that will automatically set temperature over time.
- Added `Override` methods that allow you to override the corresponding dynamic property, such as `temperatureDefinition.OverrideTemperature` or `AltosWindZone.OverrideWindSpeed`.
- Added support to release these overrides using the paired `Release` method, like `AltosWindZone.ReleaseSpeedOverride`.

## [7.4.0] - 2023-10-10

### Added

- Added a demo water shader with integration with Altos fog.
- Added a demo terrain shader with integration with Altos temperature.

## [7.3.1] - 2023-10-09

### Fixed

- Fixed an issue with temporal anti-aliasing

## [7.3.0] - 2023-10-04

### Added

- Public API for Cloud Floor and Ceiling.
- Rain and Snow VFX stop spawning when above the cloud layer.
- Added DaytimeFactor option to the WeatherEffect VFX handler.

### Changed

- Changed cloud rendering algorithms.
- Clouds are much darker when it's raining.
- World color is darker when it's raining or cloudy.
- Rain uses DaytimeFactor to be less bright at night.
- Improved rendering quality.

## [7.2.3] - 2023-09-30

### Added

- Added options to toggle the Precipitation and Origin Management options to the WeatherEffect component.

### Fixed

- WeatherEffect now handles missing or invalid properties more gracefully.

## [7.2.2] - 2023-09-28

### Added

- Added options to require a reflection trigger for position and/or time updates in the Reflection Baker.

## [7.2.1] - 2023-09-28

### Fixed

- Fixed issue with Floating Origin support for WeatherEffects
- Fixed an issue where the default setting for Rain and Snow Precipitation was 1. It is now defaulted to 0.
- Fixed an issue where the WindZone was trying to access the Sky Director before a reference was ready.

### Removed

- Removed some obsolete project files

## [7.2.0] - 2023-09-27

This version of Altos is compatible with Unity 2022.3.0f1.

### Added

- Added support for Floating Origin to WeatherEffect component.
- Added a spherical ring buffer system to Rain and Snow VFX.
- Altos Sky Director now records the origin.
- Added getter method for Altos' origin offset value, `AltosSkyDirector.Instance.GetOrigin()`.
- Added demo object for the `Disable Altos Rendering` component.

### Changed

- Improved various visual qualities of the Rain and Snow VFX.

### Fixed

- Fixed an issue with Star Daytime Brightness option.

## [7.1.0] - 2023-09-22

This version of Altos is compatible with Unity 2022.3.0f1.

### Added

- Added support for cloud types

### Changed

- Various cloud map improvements
- Weathermap UI improvements
- Cloud Definition inspector improvements
- Migrated clouds from 4-channel to single-channel textures to save on memory
- Cloud rendering algorithm adjustments

## [7.0.2] - 2023-09-20

This version of Altos is compatible with Unity 2022.3.0f1.

### Changed

- Standardized behavior of AltosWindZone to get the AltosSkyDirector Instance like the WeatherEffect and VFXSetTimeOfDay components.

### Removed

- Removed references to AltosSkyDirector from WeatherEffect and VFXSetTimeOfDay. These now pick up the Sky Director automatically.

## [7.0.1] - 2023-09-19

This version of Altos is compatible with Unity 2022.3.0f1.

### Fixed

- Fixed an issue where Sky Objects ignored "No Texture" option after setting a texture.

## [7.0.0] - 2023-09-18

This version of Altos is compatible with Unity 2022.3.0f1.

## [7.0.0-beta.3] - 2023-09-18

### Changed

- Renamed the Wind Zone component menu item from Wind Zone to Altos Wind Zone.
- The VFX Set Time of Day component will now attempt to get the SkyDirector.Instance.
- The AltosWindZone.cloudModifier and .vfxModifier properties are now public.

## [7.0.0-beta.2] - 2023-09-15

### Changed

- The altos_daytimeFactor now returns 0 at midnight and 1 at noon. For usage, Remap altos_daytimeFactor [x,y] -> [0,1], then clamp the result between 0 and 1, then run it into a Lerp.

## [7.0.0-beta.1] - 2023-09-15

### Fixed

- Fixed a bug causing runtime errors - Packages\com.occasoftware.altos\Runtime\Components\VFXLightningEffectSpawner.cs(152,51): error CS0103: The name 'm_PrefabToSpawn' does not exist in the current context.

## [7.0.0-alpha.11] - 2023-09-14

### Fixed

- Fixed an issue causing Missing Reference Exceptions when creating a Prefab from a SkyObject.

## [7.0.0-alpha.10] - 2023-09-14

### Fixed

- Fixed an issue causing the SkyObject to require a Light Component.

## [7.0.0-alpha.9] - 2023-09-14

### Added

- Added an actual Shooting Star VFX
- Added Wind Zone integration for Weather Effect
- Added a 'Current Time' property in the Sky Definition so that you can adjust the in-editor time for testing without modifying your project's initial time on game start.
- Added a slider for the current time in the Altos Sky Director component, so you no longer need to open the Skybox Definition to tab through time in editor.

### Changed

- Renamed the Shooting Star to Comets
- Reworked the Cloud Shadows system, which now uses a fixed origin and temporal AA to reduce jittering.

## [7.0.0-alpha.8] - 2023-09-13

### Added

- Added Rain and Snow VFX. These effects are integrated with the Precipitation System.
- Added Lightning VFX. This effect is integrated with the Clouds system. It is not yet integrated with the Precipitation System - Lightning zones need to be placed manually (for now).
- Added Shooting Stars VFX.
- Added a global shader variable, altos_daytimeFactor. Range [0,1]. This variable is set to 0 at night and 1 during the day. This variable enables easier integrations with external VFX and shaders. It is also available in the public API from an instance of AltosSkyDirector.daytimeFactor.

### Changes

- Moved all demo-related content to the Demo object in the AltosDemo_P scene.
- The VFX has dependencies on Visual Effect Graph, so the package.json now requires com.unity.visualeffectgraph >= 14.0.7

## [7.0.0-alpha.7] - 2023-09-11

### Added

- Added Aurora VFX

### Changed

- Changed Sun and Moon positioning system, removing the strict hierarchy requirements for these sky objects.

## [7.0.0-alpha.6] - 2023-09-07

### Added

- Added integration with Wind Zones
- Added API for Weather Zones

## [7.0.0-alpha.5] - 2023-08-29

### Added

- Added orientation offset for Sky Objects.

### Changed

- Various improvements to the Cloudmap Preview editor window.

### Fixed

- Fixed Cloudmap compatibility with Floating Point Offset system.

## [7.0.0-alpha.4] - 2023-08-28

### Removed

- Removed a bunch of unnecessary console logs.

## [7.0.0-alpha.3] - 2023-08-28

### Fixed

- Fixed an issue that caused the first cloud render to look bad in built player.

## [7.0.0-alpha.2] - 2023-08-28

### Fixed

- Fixed an issue that caused the first cloud render to look bad in Editor.

## [7.0.0-alpha.1] - 2023-08-21

### Changed

- Version to 7.y.z to reflect major changes in the recent updates.

## [6.2.0-alpha.8] - 2023-08-18

### Fixed

- Fixed an issue where clouds rendered behind geometry
- Fixed an issue where Buto fog was applied after Atmospheric fog on clouds, causing the clouds to render incorrectly.
- Fixed an issue with an unpaired Profiler.EndSample() call.

### Changed

- Tightened the edge detection to improve performance.

## [6.2.0-alpha.7] - 2023-08-17

### Added

- Added edge detection + high-resolution edge rendering

### Changed

- Cleaned up various files and shaders
- Moved render passes and other classes to standalone files
- Changed Buto integration so that Altos determines if the necessary Buto packages are installed.

## [6.2.0-alpha.6] - 2023-08-10

### Changed

- During play mode, the Sky config file shows the current time rather than the Initial Time so that you can scrub to a specific time during runtime.

### Fixed

- Fixed an error in the Weathermap update during Builds. This error was caused because the Cloudmap Shader was not included in a Resources folder or in the Altos Data asset. I added it to the data asset.
- Fixed an instance of GC Allocation from constructing Action<> reference dynamically in the Weathermap Update method. The Weathermap class now sets up the Action<> reference during the constructor to avoid the GC Alloc.

## [6.2.0-alpha.5] - 2023-08-10

### Fixed

- Fixed a warning related to shader models below target 4.5.
- Fixed a potential error for the Transparency Shader example when Buto is not available in the project.

## [6.2.0-alpha.4] - 2023-08-08

### Added

- Added a "Precipitation" system. You control this from the Weather Manager component and from the Sky Director. It uses a Render Texture to create a Weather Map each frame. This Weather Map includes (1) where clouds are, and (2) the precipitation intensity at each position. You can control the global precipitation from the Sky Director and the local precipitation from the Weather Manager (intensity and radius). You can control the cloud distribution on the weather map using the Cloud Definition -> Procedural Weathermap Type. You can preview this by opening a new Editor Window from the menu bar: OccaSoftware -> Altos -> Cloudmap Preview.
- Added a Rain prefab which demonstrates usage of the Weather Effect component. This component samples the precipitation intensity from the weather map and sets the rain particle system properties accordingly (if you are in a rainy spot, the the rain will emit particles).
- Added some shader logic in the cloud renderer that causes clouds in a precipitation area to be denser and look darker.

### Changed

- Changed the cloud upscaling algorithm to (hopefully) improve visual quality. Thoughts?
- Changed demo scene based on real-world terrain data for better reference scale.
- Changed some rendering algorithms to improve visual quality.

### Fixed

- Fixed a memory leak issue related to star compute buffers
- Fixed a memory leak issue related to release of RTHandle data

### Removed

- Removed Arbitrary Weather Effects system
- The Texture Weather Map type is temporarily not working while developing the procedural weather map system.

## [6.2.0-alpha.3] - 2023-07-28

### Added

- Added support for origin shifting (e.g., for floating point offset origin shifts).

### Changed

- Cloud positions are now handled on the CPU so that you can sync cloud positions across multiplayer, for example. This also makes it possible to change the wind velocity for any of the cloud textures during runtime.
- Cloud position and scale calculations reworked to be more responsive across-the-board with more sensible defaults.
- Cloud scale calculation reworked so that changing cloud scale during runtime looks better.

## [6.2.0-alpha.2] - 2023-07-26

### Added

- Added adjustment options for Sky Object automatic color and intensity
- Added transparency support for Altos atmospheric fog. See Altos/Samples/Transparency in the Project for example Shader and material usage.

### Fixed

- Renamed a number of variables that had similar names across Altos + Buto to avoid version-dependent compatibility issues.

## [6.2.0-alpha.1] - 2023-07-26

### Added

- Added experimental weather system.
- Added compatibility for Buto fog (requires Buto 7.0.0+).

### Fixed

- Fixed an issue causing some shaders to render improperly due to Pass and SubShader tag configuration.
- Fixed an issue causing incompatibility with some shaders in Deferred rendering mode.
- Fixed an issue where cloud volume textures didn't load correctly in builds.

## [6.1.4] - 2023-07-06

This version of Altos is compatible with Unity 2022.3.0f1.

### Changed

- Cleaned up add component menu
- Rearranged folder hierarchy

### Fixed

- Fixed default ground color settings for newly initialized Sky definition.

## [6.1.3] - 2023-06-22

This version of Altos is compatible with Unity 2022.3.0f1.

### Fixed

- Fixed various issues with cloud shadow rendering

### Removed

- Removed Cloud Shadow Temporal Anti-Aliasing (unused).

## [6.1.2] - 2023-06-22

This version of Altos is compatible with Unity 2022.3.0f1.

### Fixed

- Fixed an issue with atmospheric fog blending
- Fixed an issue with Camera Motion Vectors
- Fixed various issues with the quick start setup using the Altos -> Sky Director context menu.

## [6.1.1] - 2023-06-21

This version of Altos is compatible with Unity 2022.3.0f1.

### Fixed

- Fixed an issue causing the Background Only cloud option to render incorrectly.

### Changed

- Changed default values for definitions on setup.

## [6.1.0] - 2023-06-21

This version of Altos is compatible with Unity 2022.3.0f1.

### Added

- Altos automatically sets up new definition assets in a local folder when created using the context menu.

### Fixed

- Fixed an issue causing Altos to throw errors when the Sky Director is created using the context menu.

### Changed

- Adjusted some default settings.

## [6.0.0] - 2023-06-21

This version of Altos is compatible with Unity 2022.3.0f1.

### Added

- Added step count control for cloud shadows
- Added Cloud Shadow Cascade Shadow Maps

### Changed

- Migrated to RTHandles and Blitter API
- Overhauled Cloud Shadow system
- Changed Cloud Shadow Strength to affect the cloud density directly
- Various performance improvements

### Fixed

- Fixed motion vector algorithm

## [5.0.1] - 2023-06-22

### Fixed

- Fixed an issue with atmospheric fog blending
- Fixed an issue with Camera Motion Vectors

## [5.0.0] - 2023-06-14

This version of Altos is compatible with Unity 2021.3.0f1.

### Added

- Added star daytime brightness option
- Added more weathermap options

### Changed

- Migrated to UPM-style package
- All Shader Graphs converted to Custom Shaders
- Migrated to cmd.DrawMesh method

### Fixed

- Various bug fixes

## [4.7.5] - 2023-05-06

- Fixed an issue causing the foldouts in the Skybox Definition Editor to be unresponsive.

## [4.7.4] - 2023-04-27

- Changed cloud rendering algorithm to improved cloud rendering quality. Clouds will no longer have unusual sharp edges.
- Changed cloud rendering algorithms to improve performance.
- Fixed an issue with rendering when Depth Priming Mode was set to Forced.
- Changed the names of some cloud-related properties.
- Added some tooltips for cloud-related properties.

## [4.7.3] - 2023-04-18

- Changed some lower bound properties in the core cloud raymarch
- Changed the sky texture used by the atmospheric fog system so that it excludes sky object sky lighting.
- Changed the sky texture to use a lower resolution buffer.
- Changed some default settings.

## [4.7.2] - 2023-03-17

- Fixed an issue causing Volumetric Clouds to be disabled when Cloud Shadows was disabled.

## [4.7.1] - 2023-03-15

- Changed the Altos Render Pass and Altos Sky Director classes to avoid runtime GC Allocations. Altos now creates 0B of runtime GC Allocations (down from ~640B per frame).
- Changed the Altos Render Pass to avoid FindObjectOfType<> calls in order to improve asset performance, especially in scenes with a large number of game objects.
- Fixed an issue causing Volumetric Clouds to be disabled when Cloud Shadows was disabled.
- Fixed an issue causing the Altos Skybox to render in material previews.

## [4.7.0] - 2023-02-24

- Added compatibility for cloud shadows when using standalone cloud rendering.
- Added guards against null reference exceptions in the callbacks demo.
- Improved performance.
- The SkyObjects and Sun Object lists are now handled through the Altos Sky Director.

## [4.6.0] - 2023-02-10

- Added callbacks for period changeover, day changeover, and hour changeover, accessible from the SkyDefinition scriptable object.
- Two new properties added for the environmental lighting options: Exposure and Saturation, giving you more control over your environmental lighting.
- The Skybox Definition editor now has foldout groups and each group is indented for easier navigation.
- The cloud definition tab buttons have been moved to a horizontal layout group that looks more like tabs and indicates which tab you are currently viewing.
- Improved editor experience with time of day keyframes indented under the name tag.

## [4.5.1] - 2023-01-24

- Fixed a bug that resulted in incorrect Decal lighting.

## [4.5.0] - 2023-01-20

- Improved cloud shadow rendering.
- The atmosphere shader now supports up to 8 sky objects affecting the atmosphere lighting state. Control the intensity of the sky object atmosphere shading using the falloff setting on the associated sky object.
- Improved star rendering.
- Added additional star textures.
- You can now use the Altos clouds alone in the scene without the other components.
- Improved overall performance and code quality.

## [4.4.0] - 2023-01-13

- Improved cloud upscaling algorithm
- You can now disable Altos for specific cameras in scene. This can be useful for Overlay, UI, or Effects-related cameras. To disable Altos for a specific camera, simply add the DisableAltosRendering component to that camera.

## [‍4.3.1] - 2023-01-12

- Fixed some null reference exception errors that could occur under unusual circumstances.

## [4.3.0] - 2022-12-20

- The SkyboxDefinition now has two public methods, SetSystemTime andSetDayAndTime, that allow you to directly set the current time of day from a script.

## [4.2.0] - 2022-12-19

- You can now customize the number of stars.

## [4.1.0] - 2022-12-12

- Added more parameters to control star rendering.
- Automatic star color is now an optional setting.
- Automatic star brightness is now an optional setting.
- You can now customize the star color for both automatic and non-automatic settings.
- You can now tilt the star sphere.
- You can now lock the star sphere in place.
- Added more tooltips and summaries for various properties.
- Added a cheap ambient lighting mode option.

## [4.0.3] - 2022-12-09

- Fixed an issue with the sky objects failing to respect the time of day setting during gameplay.
- Added a tracker for the number of days elapsed: SkyDefinition.CurrentDay
- Cleaned up the Sky Director, moving multiple methods to the Sky Definition.
- Cleaned up the logic for the active time of day, adding in a SkyDefinition.timeSystem variable instead that is evaluated at runtime for SkyDefinition.CurrentTime and SkyDefinition.CurrentDay values.

## [4.0.2] - 2022-12-07

- Altos now renders correctly when using the Deferred Render Path.

## [4.0.1] - 2022-12-07

- Fixed an issue with incorrect depth testing during rendering.
- This was an issue with assigning the depth target when writing to a temporary render target within a command buffer.

## [4.0.0] - 2022-12-05

- Cloud shadow rendering
- Revamped sun positioning system
- Revamped planets and moons system
- Revamped star rendering system
- Revamped global illumination system
- Revamped atmospherics system

## [3.1.0] - 2022-09-25

- Depth Fog now renders before transparents and before the cloud pass. This should improve compatability with transparent objects in scene and should avoid inconsistent shading on opaque objects behind volumetric clouds.
- Unclamped Depth Fog Start and End values, giving more flexibility over how depth fog appears in your scene.
- Periods of Day are now treated as Keyframes. The skybox will linearly interpolate from the current keyframe to the next keyframe. This gives more control over the skybox color settings and removes some bandaids included to compensate for the old approach.

## [3.0.0] - 2022-09-22

### Volumetric Cloud Improvements

- Improved volumetric cloud lighting and density methods. Clouds will now render more realistically and with greater detail.
- The Cloud Sun Color is now a Sun Mask Color. The clouds will use the scene sun color and multiply it by the mask color. This improves the rendering quality by improving the consistency of scene-cloud lighting.
- Cloud Render Texture sampling and upscaling is now performed using a dithered min depth coverage system. This helps to prevent incorrect depth samples from appearing in the Cloud Upscaling results. These incorrect depth samples would appear as black artifacts around scene objects, especially under camera or object motion.
- Removed the Custom render scale option. This option is incompatible with the new dithered depth coverage system.
- Local Rendering is now limited to Half or Full render scales.
- Skybox Rendering still supports Quarter, Half, or Full res render scales.
- Temporal Anti-Aliasing is now performed after upscaling. This helps the TAA texture retain higher-quality data from frame-to-frame and improves the final render quality.
- Ray Depth now correctly accounts for the ray direction. Clouds in the edge of the screen will no longer fade out of view early.
- Procedural cloud weathermaps are now generated with more octaves, leading to a higher degree of quality in weathermap coverage results.
- Replaced all Volume Textures with newly created higher quality Volume Textures and simplified the Volume Texture loading system. These Volume Textures also now support mipmaps.
- Clouds are now sampled using a mipmap approach; clouds further away will be sampled using lower-detail mip levels than those nearby.
- High Altitude Clouds now rendering using multiple sampling steps, resulting in high altitude clouds that appear more 3-dimensional. These clouds now also sample lighting through each of these steps, effectively mimicking the approach used by the real low altitude volumetric clouds
- Improved the Ambient Lighting model so that clouds now have more apparent depth and detail from ambient lighting.
- Improved the Outscattering Lighting model so that clouds now have more accurate outscattering light results.
- Improved the Direct Lighting model so that clouds now capture more light deeper into the cloud volume, yielding smoother and more realistic cloud lighting results.
- Improved the Inscattering Lighting model so that clouds now more accurately demonstrate the effects of in-scattering in dense regions.
- Improved the Fog Lighting model so that low altitudue and high altitude clouds are fogged according to a shared fog density property.
- Curl Noise will now be applied on both the xz and xy axes, resulting in improved curl definition and results.
- High altitude clouds sampling method has been adjusted. Before, you had 3 noise texturues that combined for the high altitude cloud covearge. Now, you have a Weathermap + 2 noise textures that erode from the weathermap for the final density.
- Adjusted Henyey-Greenstein methods to yield more consistent and realistic results.
- High Altitude Clouds are now sampled on their own cloud layer, adding parallax and depth to the cloud coverage.
- Additional various performance optimizations.

### Sun Lamp and Day/Night Cycle Improvements

- The Sun Color is now set automatically by default based on the sun direction. The sun color and intensity is based on physically-based color temperature and intensity measurements. This improves the rendering quality by more closely replicating real light conditions based on the time of day.

### Animated Skybox Improvements

- The Skybox Sun Color is now a Sun Mask Color. The skybox will use the scene sun color and multiply it by the mask color. This improves the rendering quality by improving the consistency of scene-skybox lighting.

### Utilities and Demo Scene Improvements

- Added additional objects in the demo scene to help demonstrate the Cloud and lighting systems.
- Added a script that will monitor your trailing average frame timing to help you more easily track performance impact from various cloud features.
- Added an elastic bounce script that will automatically move some test objects in the demo scene to help observe various temporal artifacts.
- Renamed the folder that contains options to create various sky Definition assets from *Skies* to *Altos*.

## [2.2.0] - 2022-08-26

### Bug Fixes

- Skybox cloud speed will no longer increase indefinitely.

### Improvements

- Volumetric Cloud lighting has been improved.
- Volumetric Clouds can now be controlled with a density curve.

### Useful Additions

- Now includes a free camera script to simplify demo usage.

## [2.1.2]

### Bug Fixes

- Altos will now pull in a cloud volume even if it is only made available after an additive scene load.

## [2.1.1]

### Bug Fixes

- Updated Render Passes so that they no longer call the CameraColorTarget outside of the RenderPass itself.

### Housekeeping

- Added a warning that will appear on versions of Unity prior to the 2021 LTS to help ensure users are aware when they are running Altos on an incompatible version of Unity.
- Renamed some Render Passes classes to more accurately reflect the function of each class.
- Removed now-unnecessary Version Defines from the Assembly Definition.

## [2.1.0]

### Major Update

- The Altos 2022 Major Upgrade starts with release 2.1.0.

### Editor Version Update

- Altos will now be mainlined against 2021 LTS. All future releases will target 2021 LTS.

### Bug Fixes

- The cloud layer will no longer attempt to integrate with TAA Results during the first rendering frame. This improves first startup integration speed and screenshot quality.

## [2.0.0]

### Namespace Update

- Changed namespace to be more consistent with other OccaSoftware products, which use the following namespace format: OccaSoftware.Productname. The new namespace for the core functions for this asset is now OccaSoftware.Altos. The editor namespace is now OccaSoftware.Altos.Editor. I don't expect future breaking namespace changes in this or other packages. This release is labelled as a major change for this reason.

### Bug Fixes

- Custom Passes within Altos no longer attempt to run during Reflection Probe camera passes. This resolves an issue causing Reflection Probes to return black when Altos custom passes were running.
- Removed an unnecessary Motion Vectors flag setting from the Volumetric Cloud Volume script resulting in Console Warnings.

## [1.11.4]

### Bug Fix

- Corrected a TAA rendering issue that would crop up when multiple viewports were open simultaneously in the editor.

## [1.11.3]

### Bug Fix

- Corrected a Render Texture sampling issue that was causing Motion Vector-based reprojection to swim or distort on the screen in some cases.

## [1.11.2]

### Bug Fix

- Corrected the Altos.Editor assembly definition so that Editor-only files are correctly excluded from builds.

## [1.11.1]

### New Features

- Introduced Procedural Weathermaps, which dramatically improve the overall structure of the cloud distribution. These come with new parameters to give you more control over your weathermaps, such as: Weathermap Velocity, Weathermap Scale, and Weathermap Value Range.
- Texture-based Weathermaps can now pan through space, with the introduction of the Weathermap Velocity parameter for Texture-based Weathermaps.
- Improved High Altitude Cloud Rendering
- Added another 2D Cloud texture that will now be the default for Skybox and High Altitdue Clouds.

## [1.10.1]

### New Features

- Introduced Camera Motion Vector-based Reprojection for Temporal Anti-Aliasing.

## [1.9.7]

### Bug Fixes

- Fixed an issue where the Render Features would attempt to execute even in scenes without a Skybox Director present. Going forward, Skybox Director must be present in any scene where you want any Altos Render Features to execute. These Render Features subscribe to the scene changed function and check the active scene upon change for their corresponding base class to be present (Star Object, Moon Object, Cloud Volume, Time of Day Manager). If the base class is not present at the time of scene load and is injected later, you will need to manually refresh the render features to ensure they pick up their references.
- Fixed an issue where distant clouds would pop out of view when entering the cloud layer from below. Distant clouds now render correctly at all times.
- Added VFX Graph define to eliminate errors experienced when importing Altos into a project where VFX Graph is not present. However, it is critical that you import VFX Graph first, before importing the Altos asset to your project. If you have imported Altos prior to importing VFX Graph, delete your OccaSoftware/Altos file path and re-import the asset so that VFX Graph can correctly configure the asset on import. If you do not do this, the asset will throw errors.
- Cleaned up all scripts to be correctly scoped within publisher namespace (OccaSoftware).

## [1.9.6]

### Bug Fixes

- Corrected a namespace access issue.

## [1.9.5]

### Bug Fixes

- Fixed a star, moon, and cloud rendering issue for URP 12+. Note that this fix comes with the introduction of an Assembly Definition for the Altos folder path.
- Fixed a type redefinition in the Clouds HLSL.hlsl definition that resulted in shader errors in Unity 2021.2+.

## [1.9.4]

### Bug Fixes

- Fixed a star rendering issue for Unity 2020.3+

### Minor Adjustments

- Renamed Readme + Main Folder consistent with name change from Skies to Altos

## [1.9.3]

### Minor Adjustments

- Fixed cloud rendering issue for Unity 2020.3+
- Added Weathermap System.
- This system enables users to dramatically increase level of detail of cloud data by predefing cloud "zones" and using volume textures to add detail. This is in contrast to the prior system, where the base volume texture itself was used to define cloud zones, thereby failing to fully take advantage of the value of the 3D texture data.
- Added RenderScale, Planet Radius, and Volume Channel Influence Falloff default options
- Sterilized tiling values for Skybox Clouds
- Added 6 new seamless tiling textures
- 5 abstract noise textures (Clouds, Fire, Flower, Waves, Petals)
- 1 Weather Map texture (Weather Map 1)
- Cleaned up file hierarchy
- Various performance improvements

## [1.9.2]

### Minor Adjustments

- Renamed from Skies to Altos
- Added in-scattering support for clouds
- Extended some editor options
- Slightly adjusted cloud modeling formulas
- Replaced the HG Forward/Back Blend term with an HG Max term that chooses the greater of the forward and back terms.
- Changed sun shape definition formula for skybox
- 3D Textures now correctly include an alpha channel

## [1.9.1]

### Minor Fixes

- Added horizon cutoff (clouds no longer render below the horizon)
- Clamped Henyey-Greenstein Coefficients to prevent clipping artifacts at high values
- Corrected High Altitude Cloud Rendering formulas
- Added lighting support for high altitude clouds

## [1.9.0]

### Revisited Volumetric Cloud Rendering Systems

- Clouds now render using physically based lighting and rendering algorithms.
- Clouds now are configured using a completely standalone editor using a Cloud Definition construct.
- Clouds come with 3 Volumetric Noise types (Perlin, Perlin-Worley, Worley) and 4 Volumetric Noise Quality levels (Low, Medium, High, Ultra)
- Clouds now supports low-altitude volumetric rendering with up to 3 levels of detail (Base, Detail 1, Detail 2) and high-altitude 2d rendering (3 textures) along with the pre-existing skybox cloud rendering (2 textures).
- Clouds also support Temporal Anti-Aliasing, full control over render scale (continuous from 0.1x to 1x)
- Clouds now also support Skybox OR Local Rendering, allowing the clouds to be rendered in front of opaque objects in the scene.

### Revisited Star and Moon Rendering Systems

- Stars now render more accurately.
- The moon is now rendered in full 3d
- The moon now supports physically accurate and controllable phases
- The moon is now rendered using a rendering pass within the Stars Rendering Feature. The Stars Rendering Feature now acts as a generic Celestial Bodies Rendering Feature.
- Stars are defined using a Star Definition object.
- Moon is defined using a Moon Definition object.

### Performance Improvements

- Improved the performance of the Time of Day systems by moving several methods from Update to On-Demand functionality.

## [1.8.0]

### Feature Update

- Added controls that enable you to blend Volumetric Clouds with the Horizon to integrate with the Depth Fog functionality.
- Added parameters to give finer control over Volumetric Cloud alpha fadeout near the horizon.
- Added a Stars Rendering Feature that enables you to ensure Stars are always rendered behind Volumetric Clouds.

### Housekeeping

- Cleaned up naming conventions in some Render Features.
- Updated readme docs to reflect these new functionalities.
- Cleaned up assets that were associated with features that were simplified to no longer require direct material references.
- Fixed terrain import data so that it includes layer data needed for the demo scene.

## [1.7.0]

### Feature Update

- Added one-click setup for the hierarchy and Time Of Day Manager configuration. It is now much easier to set up this asset in new scenes.
- Sun and Moon Lamp light configuration now uses light temperature rather than light color to enable more realistic lighting results. This is optional, but enabled by default, and can be switched back to using color by disabling the "use light temperature" option on each directional light.
- Moved Stars to a persistent game object. Stars will now appear in the Scene view.

### Bug Fixes

- Fixed an issue that resulted when the Render Clouds command buffer shared the same command buffer designation as other render features.

## [1.6.0]

### Feature Update

- Added a Moon + Moon Shadowing support.
- Added horizon-blended fog into the Skybox.
- Added a Depth Fog image post effect, configurable from the Skybox Definition editor.
- Improved the quality of interaction between the Sun and the Volumetric Clouds.
- Updated file structure from Assets/Skies to Assets/OccaSoftware/Skies as per Unity Guidelines.

## [1.5.0]

#### Feature Update

- Improved stars.

## [1.4.0]

### Feature Update

- The Skybox Definition inspector will now show you the current in-game time in hh:mm:ss format to enable easier debugging.
- The Skybox Definition inspector will now cache the Time of Day and retain it upon entering and exiting playmode.

### Bug Fix

- Changes to your Skybox Definition will now be saved when exiting and re-launching the Unity Editor.
- Moved the "Volumetric Clouds Merge Pass" Shader into the /Shaders/Resources folder so that it is correctly included during a Build.

## [1.3.1]

### Bug Fix

- Volumetric Cloud Rendering now works in more recent versions of Unity.

## [1.0.0]

- Initial release
