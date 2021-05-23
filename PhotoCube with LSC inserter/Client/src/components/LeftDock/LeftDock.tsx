import React, { Component } from 'react';
import '../../css/LeftDock/LeftDock.css';
import { Filter } from '../Filter';
import { DateTagDropdown } from './DateFilter';
import { TagSearcher } from './TagFilter';
import { TimeForm } from './TimeFilter';

/**
 * LeftDock is the left portion of the interface.
 * PhotoCubeClient.tsx contains: LeftDock, Middle and RightDock.
 */
export default class LeftDock extends Component<{
        hideControls: boolean,
        onFiltersChanged : (filters: Filter) => void,
        activeFilters: Filter[]
    }>{
    render() {
        let visibility: string = this.props.hideControls ? "hide" : "";
        return (
            <div id="LeftDock">
                <div className="year dropdown">
                <h4 className="Header">Date filter:</h4>
                <p>Year:</p>
                <DateTagDropdown className={visibility} tagsetName={"Year"} onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters}></DateTagDropdown>
                <p>Month:</p>
                <DateTagDropdown className={visibility} tagsetName={"Month (number)"} onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters}></DateTagDropdown>
                <p>Day:</p>
                <DateTagDropdown className={visibility} tagsetName={"Day within month"} onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters}></DateTagDropdown>
                </div>
                <div className="time range">
                    <h4 className="Header">Time range filter:</h4>
                    <TimeForm className={visibility} onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters}></TimeForm>
                </div>
                <div className="tag dropdown">
                    <h4 className="Header">Tag filter:</h4>
                    <TagSearcher className={visibility} onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters}></TagSearcher>
                </div>
	  		</div>
        );
        //Not in use: <BrowsingStateLoader className={classNames}/>
    }
}