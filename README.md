# ResearchProject
PhotoCube and Lifelog Data | Research Project | Autumn 2020 | MSc Software Design | IT University of Copenhagen

Contributors: Jihye Shin (@jish) | Alexandra Waldau (@alew)

This repository contains 3 directories:
- LSC-MMM Mapping
- PhotoCube with LSC inserter
- Plotting

## LSC-MMM Mapping

This directory contains Java programs and its input/output files. It transforms Lifelog Search Challenge (LSC) dataset so that it can be mapped to Multi-dimensional Media Model (M-cube).

Note that some files are not uploaded here due to the size. The excluded files are:
- LSC Visual Concept
- LSC Metadata
- lscImageTags.csv (output from Java program)
- lscImageTags_No_Metadata.csv (output from Java program)

## PhotoCube with LSC inserter

This version of PhotoCube includes 2 additional files.
- LSCDataInsertExperimenter.cs
- LSCDataInsertExperimenterRefactored.cs

The following files are modified:
- Program.cs
- ObjectContext.cs

## Plotting

This directory contains the result of the experiments and the python scripts to generate plots.