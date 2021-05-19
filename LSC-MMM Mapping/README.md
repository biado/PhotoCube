# LSC-MMM Mapping

This directory contains Java programs, that transform Lifelog Search Challenge (LSC) dataset. The goal is to map the LSC data to Multi-dimensional Media Model (M-cube).

The input files to the program are deleted from this repository based on the LSC data usage terms.

To run this program, you need have a file named `config.properties` under `src` directory. The file should have the absolute filepath for the input files (mostly from LSC data set) in `key = filepath` format. No quotation marks needed for key and filepath.
For this version, the keys are:
 - LSCVisualConcept
 - LSCMetadata
 - LSCTopic
 - manualTagSetsMD
 - JsonHierarchy
 - LSCFilename
 - ImageFeatureTop5
 - FeatureTags
 - LSCImageTagsOutput
 - UniqueTagHierarchyOutput
 - OutputFolder
 
To parse and build json, we use the gson library. Read more and find link to download here: https://github.com/google/gson