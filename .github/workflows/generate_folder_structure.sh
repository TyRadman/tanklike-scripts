#!/bin/bash
# Script to generate folder structure

# Generate folder structure
echo "/GameProject" > folder_structure.md
find . -type d | sed 's/^\./\//' | sed 's/[^\/]*\//    /g' | sed 's/\(.*\)/├── \1/' >> folder_structure.md

# Add the output to the repository
echo "Folder structure saved as folder_structure.md"
