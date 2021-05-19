import React, { useState } from 'react';
import '../css/PhotoCubeClient.css';
import LeftDock from './LeftDock/LeftDock';
import ThreeBrowser from './Middle/ThreeBrowser/ThreeBrowser';
import GridBrowser from './Middle/GridBrowser/GridBrowser';
import CardBrowser from './Middle/CardBrowser/CardBrowser';
import RightDock from './RightDock/RightDock';
import { BrowsingModes } from './RightDock/BrowsingModeChanger';
import { BrowsingState } from './Middle/ThreeBrowser/BrowsingState';
import PickedDimension from './RightDock/PickedDimension';
import CubeObject from './Middle/ThreeBrowser/CubeObject';
import { Filter } from './LeftDock/FacetedSearcher';
import Page from './Middle/ThreeBrowser/Page';
import ICell from './Middle/ThreeBrowser/Cell';

/**
 * Root component of the PhotoCubeClient application, containing LeftDock, Middle 
 * (either ThreeBrowser, GridBrowser or a CardBrowser) and RightDock.
 */
export default class PhotoCubeClient extends React.Component {
  //Component instance refferences to allow direct child method call:
  threeBrowser = React.createRef<ThreeBrowser>();
  rightDock = React.createRef<RightDock>();

  //State of PhotoCubeClient used to render different browsing modes.
  state = {
    BrowsingMode: BrowsingModes.Cube, //Check selected value in BrowsingModeChanger, or pass down prop.
    filters: [], //Needs to be part of state to rerender ThreeBrowser when changed
    latestPage: null,
    currentPage: 1,
    pageSize: 10,
    loadMore: false,
    cubeObjects: Array<{Id: number, FileURI: string}>()
  }

  threeBrowserBrowsingState: BrowsingState | null = null;
  // cubeObjects: CubeObject[] = [];

  render() {
    //Conditional rendering:
    let currentBrowser = null;
    if (this.state.BrowsingMode == BrowsingModes.Cube) {
      currentBrowser = <ThreeBrowser ref={this.threeBrowser}
        onFileCountChanged={this.onFileCountChanged}
        previousBrowsingState={this.threeBrowserBrowsingState}
        onOpenCubeInCardMode={this.onOpenCubeInCardMode}
        onOpenCubeInGridMode={this.onOpenCubeInGridMode}
        filters={this.state.filters}
        cubeObjects={this.state.cubeObjects}
        onCellsFetched={this.onCellsFetched}
        currentPage={this.state.currentPage}
        pageSize={this.state.pageSize}
        loadMore={this.state.loadMore}
        latestPage={this.state.latestPage!}
        onNewCubeObjectsAdded={this.onNewCubeObjectsAdded} />
    } else if (this.state.BrowsingMode == BrowsingModes.Grid) {
      currentBrowser = <GridBrowser cubeObjects={this.state.cubeObjects} onBrowsingModeChanged={this.onBrowsingModeChanged} onLoadMore={this.onLoadMore} />
    } else if (this.state.BrowsingMode == BrowsingModes.Card) {
      currentBrowser = <CardBrowser cubeObjects={this.state.cubeObjects} onBrowsingModeChanged={this.onBrowsingModeChanged} />
    }

    //Page returned:
    return (
      <div className="App grid-container">
        <LeftDock hideControls={this.state.BrowsingMode != BrowsingModes.Cube}
          onFiltersChanged={this.onFiltersChanged} />
        {currentBrowser}
        <RightDock hideControls={this.state.BrowsingMode != BrowsingModes.Cube}
          ref={this.rightDock}
          onDimensionChanged={this.onDimensionChanged}
          onBrowsingModeChanged={this.onBrowsingModeChanged}
          onClearAxis={this.onClearAxis} />
      </div>
    );
  }

  /**
   * Can be called from sub-components props to update the fileCount:
   */
  onFileCountChanged = (fileCount: number) => {
    if (this.rightDock.current) this.rightDock.current.UpdateFileCount(fileCount);
  }

  /**
   * Called if filters are changed in LeftDock.
   */
  onFiltersChanged = (filters: Filter[]) => {
    let callback = () => { if (this.threeBrowser.current) { this.threeBrowser.current.RecomputeCells(); } }
    this.setState({ filters: filters }, callback);
  }

  /**
   * Can be called from sub-components props to update a dimension.
   */
  onDimensionChanged = (dimName: string, dimension: PickedDimension) => {
    console.log("Dimension " + dimName + ", changed to: ");
    console.log(dimension);
    if (this.state.BrowsingMode == BrowsingModes.Cube) {
      this.threeBrowser.current!.UpdateAxis(dimName, dimension);
    }
  }

  /**
   * Can be called from sub-components to clear an axis in the ThreeBrowser.
   */
  onClearAxis = (axisName: string) => {
    console.log("Clear axis: " + axisName);
    switch (axisName) {
      case "X": if (this.threeBrowser.current) {
        this.threeBrowser.current.ClearXAxis();
        this.threeBrowser.current.RecomputeCells();
      }
        break;
      case "Y": if (this.threeBrowser.current) {
        this.threeBrowser.current.ClearYAxis();
        this.threeBrowser.current.RecomputeCells();
      }
        break;
      case "Z": if (this.threeBrowser.current) {
        this.threeBrowser.current.ClearZAxis();
        this.threeBrowser.current.RecomputeCells();
      }
        break;
    }
  }

  /**
   * Can be called from sub-components to change the current browsing mode (the middle of the interface),
   * see BrowsingModes.tsx for details.
   */
  onBrowsingModeChanged = (browsingMode: BrowsingModes) => {
    this.rightDock.current!.ChangeBrowsingMode(browsingMode);
    if (this.state.BrowsingMode == BrowsingModes.Cube) { //Going from cube to other:
      //Saving current browsingstate:
      this.threeBrowserBrowsingState = this.threeBrowser.current!.GetCurrentBrowsingState();
      this.state.cubeObjects = this.threeBrowser.current!.GetUniqueCubeObjects();
    }
    this.setState({ BrowsingMode: browsingMode });
  }

  /**
   * Can be called from sub-components to open CubeObject array in CardMode.
   */
  onOpenCubeInCardMode = (cubeObjects: CubeObject[]) => {
    console.log("Opening cube in card mode:");
    this.threeBrowserBrowsingState = this.threeBrowser.current!.GetCurrentBrowsingState();
    this.state.cubeObjects = cubeObjects;
    this.setState({ BrowsingMode: BrowsingModes.Card });
    this.rightDock.current!.ChangeBrowsingMode(BrowsingModes.Card);
  }

  /**
   * Can be called from sub-components to open CubeObject array in GridMode.
   */
  onOpenCubeInGridMode = (cubeObjects: CubeObject[]) => {
    console.log("Opening cube in grid mode:");
    this.threeBrowserBrowsingState = this.threeBrowser.current!.GetCurrentBrowsingState();
    this.state.cubeObjects = cubeObjects;
    this.setState({ BrowsingMode: BrowsingModes.Grid });
    this.rightDock.current!.ChangeBrowsingMode(BrowsingModes.Grid);
  }

  onLoadMore = () => {
    console.log("Load more button clicked.");
    this.setState({ loadMore: true }, () => {console.log("LoadMore set to: True");});
  }

  onCellsFetched = (resultPage: Page) => {
    console.log("setState(page) called.");
    console.log(resultPage);
    this.setState({ loadMore: false });
    this.setState({ latestPage: resultPage }, () => { console.log("Result page updated.");} );
  }

  onNewCubeObjectsAdded = (newCubeObjects: CubeObject[]) => {
    this.setState({ cubeObjects: newCubeObjects }, () => {console.log("CubeObject[] updated.")});
  }
}