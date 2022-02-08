import React, { Component } from 'react';
import '../../css/LeftDock/LeftDock.css';
import '../../css/PhotoCubeClient.css';
import { Filter } from '../Filter';
import { DateTagDropdown } from './DateFilter';
import DayOfWeekFilter from './DayOfWeekFilter';
import { TagSearcher } from './TagFilter';
//import { TimeFilter } from './TimeFilter';
//import { Slider } from './Slider';
import { SpotifyWidget } from './SpotifyWidget'
import { Slider } from './Slider';
import { Emotion } from './Emotion';

/**
 * LeftDock is the left portion of the interface.
 * PhotoCubeClient.tsx contains: LeftDock, Middle and RightDock.
 */
export default class LeftDock extends Component<{
        hideControls: boolean,
        onFiltersChanged : (filters: Filter) => void,
        activeFilters: Filter[],
        onFilterReplaced: (oldFilter:Filter, newFilter: Filter) => void,
        onFilterRemoved: (filterId: number) => void,
        onFilterReplacedByType: (oldFilter:Filter, newFilter: Filter) => void,
        onFilterRemovedByType: (filterType: string) => void
        spotifyURI: String | null
    }>{
    render() {
        let visibility: string = this.props.hideControls ? " hide" : "";
        return (
            <div id={"LeftDock"} >
                <div className={""}>
                    <h4 className="Header">Spotify widget:</h4>
                    <h5>Click to select track</h5>
                    <SpotifyWidget spotifyURI={this.props.spotifyURI}/>
                </div>
                <div className={visibility}>
                    <h4 className="Header">Date filter:</h4>
                     <div className="date dropdowns">
                        <DateTagDropdown tagsetName={"day"} onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters} onFilterReplaced={this.props.onFilterReplaced} onFilterRemoved={this.props.onFilterRemoved}/>
                        <DateTagDropdown tagsetName={"month"} onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters} onFilterReplaced={this.props.onFilterReplaced} onFilterRemoved={this.props.onFilterRemoved}/>
                        <DateTagDropdown tagsetName={"year"} onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters} onFilterReplaced={this.props.onFilterReplaced} onFilterRemoved={this.props.onFilterRemoved}/>
                    </div>
                </div>
                <div className={visibility}>
                    <h4 className="Header">Tag filter:</h4>
                    <TagSearcher onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters}/>
                </div>
                <div className={visibility}>
                    <h4 className="Header">Emotion code filter:</h4>
                    <Emotion onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters} onFilterRemoved={this.props.onFilterRemoved}></Emotion>
                    <DayOfWeekFilter onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters} onFilterRemoved={this.props.onFilterRemoved}></DayOfWeekFilter>
                </div>
                <div className={visibility}>
                    <h4 className="Header">Slider:</h4>
                    {/*<Slider tagsetName={"sp_track_duration"} onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters} onFilterReplaced={this.props.onFilterReplaced} onFilterRemoved={this.props.onFilterRemoved}/>*/}
                    <Slider tagsetName={"happiness_percentage"} onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters} onFilterReplaced={this.props.onFilterReplaced} onFilterRemoved={this.props.onFilterRemoved}/>
                    <Slider tagsetName={"sadness_percentage"} onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters} onFilterReplaced={this.props.onFilterReplaced} onFilterRemoved={this.props.onFilterRemoved}/>
                    <Slider tagsetName={"anger_percentage"} onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters} onFilterReplaced={this.props.onFilterReplaced} onFilterRemoved={this.props.onFilterRemoved}/>
                    <Slider tagsetName={"fear_percentage"} onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters} onFilterReplaced={this.props.onFilterReplaced} onFilterRemoved={this.props.onFilterRemoved}/>
                </div> 
            </div>
        );
    }
}
