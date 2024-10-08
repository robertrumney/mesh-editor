# Unity Mesh Editor Tool

A Unity Editor Window tool that allows you to select and remove faces of a mesh. This tool provides a graphical interface in the Unity Editor for selecting faces by clicking on them in the Scene view and saving the modified mesh as a new asset.

## Features

- Select individual faces of a mesh in the Scene view by clicking on them.
- Hold `Ctrl` and click to select all connected faces.
- Highlight selected faces in the Scene view.
- Remove selected faces from the mesh.
- Option to remove everything except the selected faces.
- Save the modified mesh as a new asset.

## Usage

1. **Attach a Mesh Collider**:
   - Ensure the GameObject you want to edit has a `MeshFilter` component with a mesh.
   - Add a `MeshCollider` component to the GameObject and ensure it uses the same mesh.

2. **Open the Tool**:
   - Go to the Unity menu bar and select `Tools > Mesh Editor`.

3. **Select Faces**:
   - In the Mesh Editor window, assign the `MeshFilter` you want to edit.
   - Click on faces in the Scene view to select them.
   - Hold `Ctrl` and click on a face to select all connected faces.
   - Selected faces will be highlighted in the Scene view.

4. **Choose Removal Mode**:
   - Toggle the "Keep Only Selected Faces" option to switch between removing selected faces and keeping only the selected faces.

5. **Apply Changes**:
   - Click the `Apply Changes` button to either remove selected faces or keep only the selected faces based on the toggle.

6. **Save the Mesh**:
   - Click the `Save New Mesh` button to save the modified mesh as a new asset.

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.
