import React from 'react';
import '../../css/RightDock/RightDock.css';
import FileCount from './FileCount';
import BrowsingModeChanger, { BrowsingModes } from './BrowsingModeChanger';
import Dimensions from './Dimensions';
import PickedDimension from './PickedDimension';
import { FilterList } from './FilterList';
import { Filter } from '../Filter';
import { HierarchyExplorer } from '../Middle/BottomDock/HierarchyFilter';

/**
 * RightDock is the right portion of the interface.
 * PhotoCubeClient.tsx contains: LeftDock, Middle, including Bottom Dock, and RightDock.
 */
export default class RightDock extends React.Component<{
        //Props contract:
        onBrowsingModeChanged:(browsingmode: BrowsingModes) => void,
        onDimensionChanged:(dimName: string, dimension:PickedDimension) => void,
        onClearAxis:(axisName: string) => void,
        onFiltersChanged : (filters: Filter) => void,
        hideControls: boolean,
        activeFilters: Filter[],
        onFilterRemoved: (filterId: number) => void
    }>{

    private fileCount = React.createRef<FileCount>();
    private browsingModeChanger = React.createRef<BrowsingModeChanger>();

    render(){
        let visibility: string = this.props.hideControls ? "hide" : "";
        let fileCountId: string = visibility + " fileCount"
        return(
            <div id="RightDock">
                {/* <BrowsingModeChanger ref={this.browsingModeChanger} onBrowsingModeChanged={this.props.onBrowsingModeChanged} /> */}
                {/* <Dimensions className={visibility} activeFilters={this.props.activeFilters} onDimensionChanged={this.onDimensionChanged} onClearAxis={this.onClearAxis}/> */}
                {/* <FilterList className ={visibility} activeFilters={this.props.activeFilters} onFilterRemoved={this.props.onFilterRemoved} /> */}
                <div className="hierarchy explorer">
                    <h4 className="Header">Hierarchy filter:</h4>
                    <HierarchyExplorer activeFilters={this.props.activeFilters.filter(af => af.type === 'hierarchy')} onFiltersChanged={this.props.onFiltersChanged}/>
                </div>
                <FileCount className={fileCountId} ref={this.fileCount}/>
            </div>
        );
    }

    onDimensionChanged = (dimName: string, dimension:PickedDimension) => {
        this.props.onDimensionChanged(dimName, dimension);
    }

    onClearAxis = (axisName: string) => {
        this.props.onClearAxis(axisName);
    }

    UpdateFileCount(count: number){
        this.fileCount.current!.UpdateFileCount(count);
    }

    ChangeBrowsingMode = (browsingMode:BrowsingModes) => {
        this.browsingModeChanger.current!.ChangeSelectedBrowsingMode(browsingMode);
    }
}