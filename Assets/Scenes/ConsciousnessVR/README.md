# Autocatalytic Consciousness Visualization in VR

This project implements an immersive VR experience that visualizes Liane Gabora's autocatalytic theory of consciousness, showing how consciousness progresses from an ambient level throughout the universe to increasingly amplified states in biological systems and human cognition.

## Project Overview

The visualization consists of three nested layers:

1. **Ambient Consciousness Layer** - The outermost layer representing the baseline consciousness that exists throughout the universe, visualized as subtle wave patterns and sparse particles.

2. **Biological Consciousness Layer** - The middle layer showing how living systems amplify consciousness through autocatalytic closure, represented by a boundary with resonance rings, nodes, and self-reinforcing loops.

3. **Cognitive Consciousness Layer** - The innermost layer demonstrating the integrated worldview of human cognition, featuring a bright core, standing wave patterns, and an interconnected concept network.

## Setup Instructions

### Prerequisites

- Unity 2022.3 or newer
- XR Interaction Toolkit package
- OpenXR or other VR SDK installed

### Scene Setup

1. Open the "ConsciousnessVR" scene in Unity.
2. Ensure you have a VR headset connected to your system.
3. If using a VR system that's not supported by default, configure your system in the XR Plugin Management settings.

## Using the Visualization

### Navigation

- Use your VR controllers to move between the different layers of consciousness.
- Point and teleport to areas of interest using the standard XR Interaction ray.
- Physical movement will allow you to inspect elements more closely.

### Interaction

- **Direct Interaction**: Point at and select interactive elements to get more information about them.
- **Layer Switching**: Use the UI buttons or hand gestures to focus on specific layers.
- **Information Panels**: Interacting with layer elements will bring up information panels explaining key concepts.

### Guided Tour

1. Access the guided tour using the "Start Tour" button in the main menu.
2. Follow the narrated journey through the three levels of consciousness.
3. Navigate between tour steps using the "Next" and "Previous" buttons.
4. Exit the tour at any time with the "Exit Tour" button.

## Technical Structure

The visualization is built using several key components:

- **ConsciousnessVisualization**: Main manager script that controls the visibility and transitions between the three layers.
- **AmbientConsciousnessLayer**: Creates the ambient waves and particles of the outer layer.
- **BiologicalConsciousnessLayer**: Manages the middle layer with autocatalytic processes and self-organizing patterns.
- **CognitiveConsciousnessLayer**: Implements the integrated worldview patterns and concept networks.
- **InfoPanelSystem**: Provides educational content about the different elements and layers.
- **InteractiveElement**: Makes objects in the scene interactable in VR.
- **GuidedTourManager**: Controls the automated educational tour through the visualization.

## Educational Components

- **Layer Explanations**: Each layer comes with descriptive text and visual examples.
- **Interactive Learning**: Information panels provide quotes from Gabora's work.
- **Philosophical Context**: Special panels discuss implications of the theory.

## Troubleshooting

- If VR controllers aren't detected, ensure your VR system is properly connected and configured in Unity's XR settings.
- If the visualization appears too intensive for your system, try reducing the particle counts in the AmbientConsciousnessLayer script.
- For issues with text readability, adjust the InfoPanelSystem's follow distance parameter.

## Credits

- Concept based on Liane Gabora's theory of consciousness as autocatalytic amplification
- Developed using Unity's XR Interaction Toolkit 