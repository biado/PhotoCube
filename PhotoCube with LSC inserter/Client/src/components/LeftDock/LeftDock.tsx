import React, { Component } from 'react';
import '../../css/LeftDock/LeftDock.css';
import { Filter } from '../Filter';
import { DateTagDropdown } from './DateFilter';
import DayOfWeekFilter from './DayOfWeekFilter';
import { TagSearcher } from './TagFilter';
import { TimeForm } from './TimeFilter';

/**
 * LeftDock is the left portion of the interface.
 * PhotoCubeClient.tsx contains: LeftDock, Middle and RightDock.
 */
export default class LeftDock extends Component<{
        hideControls: boolean,
        onFiltersChanged : (filters: Filter) => void,
        activeFilters: Filter[],
        onFilterUnchecked: (filterId : number) => void
    }>{
    render() {
        let visibility: string = this.props.hideControls ? "hide" : "";
        return (
            <div className={"Left dock " + visibility} >
                <div className="time range">
                    <h4 className="Header">Day of week filter:</h4>
                    <DayOfWeekFilter onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters} onFilterUnchecked={this.props.onFilterUnchecked}></DayOfWeekFilter>
                </div>
                <div className="year dropdown">
                <h4 className="Header">Date filter:</h4>
                <p>Year:</p>
                <DateTagDropdown tagsetName={"Year"} onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters}></DateTagDropdown>
                <p>Month:</p>
                <DateTagDropdown tagsetName={"Month (number)"} onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters}></DateTagDropdown>
                <p>Day:</p>
                <DateTagDropdown tagsetName={"Day within month"} onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters}></DateTagDropdown>
                </div>
                <div className="time range">
                    <h4 className="Header">Time range filter:</h4>
                    <TimeForm onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters}></TimeForm>
                </div>
                <div className="tag dropdown">
                    <h4 className="Header">Tag filter:</h4>
                    <TagSearcher onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters}></TagSearcher>
                </div>
	  		</div>
        );
        //Not in use: <BrowsingStateLoader className={classNames}/>
    }
}