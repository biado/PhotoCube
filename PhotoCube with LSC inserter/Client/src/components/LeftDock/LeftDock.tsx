import React, { Component } from 'react';
import '../../css/LeftDock/LeftDock.css';
import { Filter } from '../Filter';
import { DateTagDropdown } from './DateFilter';
import DayOfWeekFilter from './DayOfWeekFilter';
import { TagSearcher } from './TagFilter';
import { TimeFilter } from './TimeFilter';

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
    }>{
    render() {
        let visibility: string = this.props.hideControls ? "hide" : "";
        return (
            <div id={"LeftDock" + visibility} >
                <div>
                    <h4 className="Header">Day of week filter:</h4>
                    <DayOfWeekFilter onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters} onFilterRemoved={this.props.onFilterRemoved}></DayOfWeekFilter>
                </div>
                <div>
                    <h4 className="Header">Date filter:</h4>
                    <div className="date dropdowns">
                        <DateTagDropdown tagsetName={"Day within month"} onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters} onFilterReplaced={this.props.onFilterReplaced} onFilterRemoved={this.props.onFilterRemoved}/>
                        <DateTagDropdown tagsetName={"Month (number)"} onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters} onFilterReplaced={this.props.onFilterReplaced} onFilterRemoved={this.props.onFilterRemoved}/>
                        <DateTagDropdown tagsetName={"Year"} onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters} onFilterReplaced={this.props.onFilterReplaced} onFilterRemoved={this.props.onFilterRemoved}/>
                    </div>
                </div>
                <div>
                    <h4 className="Header">Time range filter:</h4>
                    <TimeFilter onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters} onFilterReplacedByType={this.props.onFilterReplacedByType} onFilterRemovedByType={this.props.onFilterRemovedByType}/>
                </div>
                <div>
                    <h4 className="Header">Tag filter:</h4>
                    <TagSearcher onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters}/>
                </div>
	  		</div>
        );
    }
}