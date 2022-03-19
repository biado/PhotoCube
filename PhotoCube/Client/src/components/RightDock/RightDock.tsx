import React from 'react';
import '../../css/RightDock/RightDock.css';
import FileCount from './FileCount';
import BrowsingModeChanger, { BrowsingModes } from './BrowsingModeChanger';
import Dimensions from './Dimensions';
import PickedDimension from './PickedDimension';
import { FilterList } from './FilterList';
import { Filter } from '../Filter';
import { SpotifyWidget } from '../LeftDock/SpotifyWidget';
import { ColorToggle } from './ColorToggle';

/**
 * RightDock is the right portion of the interface.
 * PhotoCubeClient.tsx contains: LeftDock, Middle, including Bottom Dock, and RightDock.
 */
export default class RightDock extends React.Component<{
        //Props contract:
        onDimensionChanged:(dimName: string, dimension:PickedDimension) => void,
        onBrowsingModeChanged:(browsingmode: BrowsingModes) => void,
        onClearAxis:(axisName: string) => void,
        hideControls: boolean,
        hideCount: boolean,
        activeFilters: Filter[],
        onFilterRemoved: (filterId: number) => void,
        // spotifyURI: String | null,
        onColorChange: (color:boolean) => void,
    }>{

    private fileCount = React.createRef<FileCount>();
    private browsingModeChanger = React.createRef<BrowsingModeChanger>();

    render(){
        let visibility: string = this.props.hideControls ? "hide" : "";
        let countVisibility: string = this.props.hideCount ? "hide" : "";
        return(
            <div id="RightDock">
                <FileCount className={countVisibility} ref={this.fileCount} />
                <ColorToggle className={visibility} onColorChange={this.props.onColorChange}></ColorToggle>
                {/* <div className={"spotify_widget"}>
                    <SpotifyWidget spotifyURI={this.props.spotifyURI}/>
                </div> */}
                <BrowsingModeChanger ref={this.browsingModeChanger} onBrowsingModeChanged={this.props.onBrowsingModeChanged} />
                <Dimensions className={visibility} activeFilters={this.props.activeFilters} onDimensionChanged={this.onDimensionChanged} onClearAxis={this.onClearAxis}/>
                <FilterList className ={visibility} activeFilters={this.props.activeFilters} onFilterRemoved={this.props.onFilterRemoved} />
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
