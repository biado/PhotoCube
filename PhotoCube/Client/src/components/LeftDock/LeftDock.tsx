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
import { Operation } from './Operation';

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
        state = {
            showSlider : [false, false, false, false]
        }

    changeSliders(num: number) {
        console.log("visible emotion",num)
        var values = this.state.showSlider
        values[num] = ! values[num] 
        this.setState( {showSlider : values} )
    }

    render() {
        let visibility: string = this.props.hideControls ? " hide" : "";
        return (
            <div id={"LeftDock"} >
                <div className={"spotify_widget"}>
                    <h4 className="Header">Spotify widget:</h4>
                    <SpotifyWidget spotifyURI={this.props.spotifyURI}/>
                </div>
                <div className={visibility}>
                    <h4 className="Header">Track duration:</h4>
                    <Slider tagsetName={"sp_track_duration"} rangeDirection={Operation.LessThanOrEqual} onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters} onFilterReplaced={this.props.onFilterReplaced} onFilterRemoved={this.props.onFilterRemoved}/>
                </div>
                <div className={visibility}>
                    <h4 className="Header">Release date filter:</h4>
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
                    <DayOfWeekFilter visibleSlider={(n)=>this.changeSliders(n)} onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters} onFilterRemoved={this.props.onFilterRemoved}></DayOfWeekFilter>
                    {(this.state.showSlider[0]) ? <Slider tagsetName={"happiness_percentage"} rangeDirection={Operation.GreaterThanOrEqual} onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters} onFilterReplaced={this.props.onFilterReplaced} onFilterRemoved={this.props.onFilterRemoved}/> : <span/>}
                    {(this.state.showSlider[1]) ? <Slider tagsetName={"sadness_percentage"} rangeDirection={Operation.GreaterThanOrEqual} onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters} onFilterReplaced={this.props.onFilterReplaced} onFilterRemoved={this.props.onFilterRemoved} /> : <span/>}
                    {(this.state.showSlider[2]) ? <Slider tagsetName={"anger_percentage"} rangeDirection={Operation.GreaterThanOrEqual} onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters} onFilterReplaced={this.props.onFilterReplaced} onFilterRemoved={this.props.onFilterRemoved}/> : <span/>}
                    {(this.state.showSlider[3]) ? <Slider tagsetName={"fear_percentage"} rangeDirection={Operation.GreaterThanOrEqual} onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters} onFilterReplaced={this.props.onFilterReplaced} onFilterRemoved={this.props.onFilterRemoved}/> : <span/>}
                </div> 
            </div>
        );
    }
}
