import React from 'react';
import '../css/PhotoCubeClient.css';
import LeftDock from './LeftDock/LeftDock';
import CubeBrowser from './Middle/CubeBrowser/CubeBrowser';
import GridBrowser from './Middle/GridBrowser/GridBrowser';
import CardBrowser from './Middle/CardBrowser/CardBrowser';
import RightDock from './RightDock/RightDock';
import { BottomDock } from './Middle/BottomDock/BottomDock';
import { BrowsingModes } from './RightDock/BrowsingModeChanger';
import { BrowsingState } from './Middle/CubeBrowser/BrowsingState';
import PickedDimension from './RightDock/PickedDimension';
import CubeObject from './Middle/CubeBrowser/CubeObject';
import { Filter } from './Filter';


interface ClientState {
  BrowsingMode : BrowsingModes.Card | BrowsingModes.Cube | BrowsingModes.Grid,
  filters: Filter[]
}

/**
 * Root component of the PhotoCubeClient application, containing LeftDock, Middle 
 * (either CubeBrowser, GridBrowser or a CardBrowser) and RightDock.
 */
export default class PhotoCubeClient extends React.Component<ClientState> {
  //Component instance refferences to allow direct child method call:
  CubeBrowser = React.createRef<CubeBrowser>();
  rightDock = React.createRef<RightDock>();
  
  //State of PhotoCubeClient used to render different browsing modes.
  state: ClientState = {
    BrowsingMode: BrowsingModes.Cube, //Check selected value in BrowsingModeChanger, or pass down prop.
    filters: [] //Needs to be part of state to rerender CubeBrowser when changed
  }

  CubeBrowserBrowsingState : BrowsingState|null = null;
  cubeObjects : CubeObject[] = [];

  render() {
    //Conditional rendering:
    let currentBrowser = null;
    if(this.state.BrowsingMode == BrowsingModes.Cube){
      currentBrowser = <CubeBrowser ref={this.CubeBrowser} 
        onFileCountChanged={this.onFileCountChanged} 
        previousBrowsingState={this.CubeBrowserBrowsingState}
        onOpenCubeInCardMode={this.onOpenCubeInCardMode}
        onOpenCubeInGridMode={this.onOpenCubeInGridMode}
        filters={this.state.filters}/>
    }else if(this.state.BrowsingMode == BrowsingModes.Grid){
      currentBrowser = <GridBrowser /* cubeObjects={this.cubeObjects} */ onBrowsingModeChanged={this.onBrowsingModeChanged}/>
    }else if(this.state.BrowsingMode == BrowsingModes.Card){
      currentBrowser = <CardBrowser cubeObjects={this.cubeObjects} onBrowsingModeChanged={this.onBrowsingModeChanged}/>
    }

    //Page returned:
    return (
        <div className="App grid-container">
          <LeftDock 
          hideControls={this.state.BrowsingMode != BrowsingModes.Cube} 
          onFiltersChanged={this.onFiltersChanged}
          activeFilters={this.state.filters}
          onFilterReplaced={this.onFilterReplaced}
          onFilterRemoved={this.onFilterRemoved}
          onFilterReplacedByType={this.onFilterReplacedByType}
          onFilterRemovedByType={this.onFilterRemovedByType}
          />
           <div className="middle dock">
            {currentBrowser}
            <BottomDock 
              hideControls={this.state.BrowsingMode != BrowsingModes.Cube} 
              activeFilters={this.state.filters} 
              onFiltersChanged={this.onFiltersChanged}/>
          </div>
          <RightDock hideControls={this.state.BrowsingMode != BrowsingModes.Cube} 
            ref={this.rightDock}
            onDimensionChanged={this.onDimensionChanged} 
            onBrowsingModeChanged={this.onBrowsingModeChanged}
            onClearAxis={this.onClearAxis}
            activeFilters={this.state.filters}
            onFilterRemoved={this.onFilterRemoved}/>
        </div>
    );
  }

  /**
   * Can be called from sub-components props to update the fileCount:
   */
  onFileCountChanged = (fileCount: number) => {
    if(this.rightDock.current) this.rightDock.current.UpdateFileCount(fileCount);
  }

  /**
   * Called if filters are changed in Left and Bottom Dock.
   */
  onFiltersChanged = (filter: Filter) =>{
    let callback = () => { if(this.CubeBrowser.current){ this.CubeBrowser.current.RecomputeCells(); }}
    this.state.filters.unshift(filter);
    this.setState({filters: this.state.filters.flat()}, callback);
  }

  /**
   * Called if a filter is removed from the list in the Right Dock.
   */
  onFilterRemoved = (filterId : number) => {
    let callback = () => { if(this.CubeBrowser.current){ this.CubeBrowser.current.RecomputeCells(); }}
    this.setState({filters : this.state.filters.filter(filter => filter.Id !== filterId)}, callback);
  }

  /**
  * Called if a filter is removed with type information.
  */
  onFilterRemovedByType = (filterType: string) => {
    let callback = () => { if (this.CubeBrowser.current) { this.CubeBrowser.current.RecomputeCells(); } }
    this.setState({ filters: this.state.filters.filter(filter => filter.type !== filterType) }, callback);
  }

  /**
  * Called if a filter is replaced.
  */
  onFilterReplaced = (oldFilter:Filter, newFilter: Filter) => {
    let callback = () => { if (this.CubeBrowser.current) { this.CubeBrowser.current.RecomputeCells(); } }
    let newFilters: Filter[] = this.state.filters.filter(filter => filter.Id !== oldFilter.Id);
    newFilters.unshift(newFilter);
    this.setState({ filters: newFilters.flat() }, callback);
  }

  /**
  * Called if a time filter is replaced.
  */
   onFilterReplacedByType = (oldFilter:Filter, newFilter: Filter) => {
    let callback = () => { if (this.CubeBrowser.current) { this.CubeBrowser.current.RecomputeCells(); } }
    let newFilters: Filter[] = this.state.filters.filter(filter => filter.type !== oldFilter.type); 
    newFilters.unshift(newFilter);
    this.setState({ filters: newFilters.flat() }, callback);
  }

  /**
   * Can be called from sub-components props to update a dimension.
   */
  onDimensionChanged = (dimName: string, dimension:PickedDimension) => {
    console.log("Dimension " + dimName + ", changed to: ");
    console.log(dimension);
    if(this.state.BrowsingMode == BrowsingModes.Cube){
      this.CubeBrowser.current!.UpdateAxis(dimName, dimension);
    }
  }

  /**
   * Can be called from sub-components to clear an axis in the CubeBrowser.
   */
  onClearAxis = (axisName: string) => {
    console.log("Clear axis: " + axisName);
    switch(axisName){
      case "X": if(this.CubeBrowser.current) {
        this.CubeBrowser.current.ClearXAxis();
        this.CubeBrowser.current.RecomputeCells();
      }
      break;
      case "Y": if(this.CubeBrowser.current) {
        this.CubeBrowser.current.ClearYAxis(); 
        this.CubeBrowser.current.RecomputeCells();
      }
      break;
      case "Z": if(this.CubeBrowser.current) {
        this.CubeBrowser.current.ClearZAxis();
        this.CubeBrowser.current.RecomputeCells();
      }
      break;
    }
  }

  /**
   * Can be called from sub-components to change the current browsing mode (the middle of the interface),
   * see BrowsingModes.tsx for details.
   */
  onBrowsingModeChanged = (browsingMode: BrowsingModes) =>{
    this.rightDock.current!.ChangeBrowsingMode(browsingMode);
    if(this.state.BrowsingMode == BrowsingModes.Cube){ //Going from cube to other:
      //Saving current browsingstate:
      this.CubeBrowserBrowsingState = this.CubeBrowser.current!.GetCurrentBrowsingState();
      this.cubeObjects = this.CubeBrowser.current!.GetUniqueCubeObjects()
    }
    this.setState({BrowsingMode: browsingMode});
  }

  /**
   * Can be called from sub-components to open CubeObject array in CardMode.
   */
  onOpenCubeInCardMode = (cubeObjects: CubeObject[]) => {
    console.log("Opening cube in card mode:");
    this.CubeBrowserBrowsingState = this.CubeBrowser.current!.GetCurrentBrowsingState();
    this.cubeObjects = cubeObjects;
    this.setState({BrowsingMode: BrowsingModes.Card});
    this.rightDock.current!.ChangeBrowsingMode(BrowsingModes.Card);
  }

  /**
   * Can be called from sub-components to open CubeObject array in GridMode.
   */
  onOpenCubeInGridMode = (cubeObjects: CubeObject[]) => {
    console.log("Opening cube in grid mode:");
    this.CubeBrowserBrowsingState = this.CubeBrowser.current!.GetCurrentBrowsingState();
    this.cubeObjects = cubeObjects;
    this.setState({BrowsingMode: BrowsingModes.Grid});
    this.rightDock.current!.ChangeBrowsingMode(BrowsingModes.Grid);
  }
}