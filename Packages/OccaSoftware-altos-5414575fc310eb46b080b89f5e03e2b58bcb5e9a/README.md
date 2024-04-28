# Altos

[![Discord](https://img.shields.io/discord/999031026204553316?logo=discord&label=discord)](https://www.occasoftware.com/discord)

[![GitHub Release](https://img.shields.io/github/release/occasoftware/com.occasoftware.altos?logo=github)](https://github.com/occasoftware/com.occasoftware.altos/releases/latest)

Altos is a complete sky system that provides a configurable skybox, weather, time of day, and visual effects.

## Getting Started

Visit the [OccaSoftware Docs Site](https://docs.occasoftware.com/) for package & API documentation.

## Initial Setup

1. Import the Altos package in your Packages folder in your project's directory
2. Add the Altos Renderer Feature.
    1. Open your UniversalRendererData asset.
    2. Click “Add Renderer Feature”
    3. Select “Altos Renderer Feature” to add the Renderer to your project.
3. Add the Altos Sky Director
    1. Navigate to your scene hierarchy
    2. Right click to open the context menu
    3. Go to the Altos folder
    4. Select Sky Director to set up a Sky Director in your project.
4. Configure the various parameters.

## Community and Feedback

For general questions, issue reporting, or support, please join our [Discord Community](https://www.occasoftware.com/discord)

## Branching Strategy

The default branch is main. That's where you can find the latest version of the package. We development branch is develop. That's where you can find in-development code.

During development, we merge PRs directly on the main branch (or maintenance branch, if applicable). The main branch and any release branches should always be shippable: the code compiles and features are complete.
