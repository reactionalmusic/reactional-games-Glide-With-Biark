# Reactional Games - Glide With Biark!

Glide With Biark is a special celebration of the 10th anniversary of Flappy Bird, showcasing the powerful capabilities of Reactional Deep Analysis by integrating music with procedural level generation and gameplay elements.

## 🎮 Overview

Glide With Biark is a music-driven endless runner inspired by games like Flappy Bird and Geometry Dash. Players must navigate through procedurally generated obstacles while staying in sync with the rhythm of the music.

### Key Features

- Procedural Level Generation:
  - Obstacles dynamically generated based on the music's instruments and beats.
  - Eliminates the need for manual level design, allowing music to dictate gameplay.
  - Players must time their movements in sync with the music to overcome challenges.

- Reactive Visual Effects:
  - Flashy shaders and visual effects tied to musical elements.
  - Patterns respond to specific instruments, creating a visually dynamic experience.
  - Deep Analysis enhances both auditory and visual feedback for immersive gameplay.

- Seamless Integration:
  - Reactional Deep Analysis drives gameplay elements directly.
  - Demonstrates how developers can integrate music into procedural mechanics effortlessly.

## 🚀 How Glide With Biark Improves on Geometry Dash

Unlike Geometry Dash, Glide With Biark leverages Reactional Deep Analysis to procedurally generate obstacles and visuals in real-time, offering a truly music-driven experience without manual level design.

## 💻 Development Information

### Dependencies

- Unity 6 LTS: Built using Unity 6 (6000.0.24f1)
- Reactional Plugin: Enables real-time music-driven level generation.

## 📂 Project Structure

- Minimal Added Mechanics: The current implementation is kept simple to highlight the potential of Deep Analysis without additional mechanics. It emphasizes procedural generation, beat-synced movement, and music-driven gameplay elements.

## 🛠️ Good to Know for Developers

### Reactional Deep Analysis Scripts

- `Reactional_DeepAnalysis_EventDispatcher`: Subscribes to the events of instruments analyzed with Deep Analysis. Each note played by an instrument triggers live events sent to subscribed functions.
- `Reactional_DeepAnalysis_PitchData`: Captures the pitch of instrument notes assigned by Deep Analysis. Useful for triggering pitch-toned sound effects based on music data.
- `Reactional_DeepAnalysis_ProceduralMapGenerator`: Instantiates and places objects at specified notes from respective instruments all at once. Utilizes BPM to dynamically move objects in sync with the music.

### Gameplay

- `Reactional_DeepAnalysis_LightAndPostController`: Subscribes to the Reactional Deep Analysis Event Dispatcher. Adjusts global light intensity to fine-tune lighting effects based on musical instruments.
- `Reactional_CameraPulseOnBeat`: Leverages Reactional's GetCurrentBeat() functionality. Modifies camera zoom to pulse in sync with the beat for enhanced immersion.


Feel free to explore and integrate these scripts into your projects to fully leverage the power of Reactional Deep Analysis.
