import React, { Component } from 'react';
import '../../css/LeftDock/LeftDock.css';
import '../../css/PhotoCubeClient.css';
import { Filter } from '../Filter';
import { DateTagDropdown } from './DateFilter';
import DayOfWeekFilter from './DayOfWeekFilter';
import { TagSearcher } from './TagFilter';
import { TimeFilter } from './TimeFilter';
import { HierarchyExplorer } from '../Middle/BottomDock/HierarchyFilter';
import { DimensionBrowser } from '../Middle/BottomDock/DimensionBrowser';
import PickedDimension from '../RightDock/PickedDimension';
import Dimensions from '../RightDock/Dimensions';
import { FilterList } from '../RightDock/FilterList';

/**
 * LeftDock is the left portion of the interface.
 * PhotoCubeClient.tsx contains: LeftDock, Middle and RightDock.
 */
export default class LeftDock extends Component<{
        hideControls: boolean,
        onFiltersChanged : (filters: Filter) => void,
        activeFilters: Filter[],
        onDimensionChanged:(dimName: string, dimension:PickedDimension) => void,
        onClearAxis:(axisName: string) => void,
        onFilterReplaced: (oldFilter:Filter, newFilter: Filter) => void,
        onFilterRemoved: (filterId: number) => void,
        onFilterReplacedByType: (oldFilter:Filter, newFilter: Filter) => void,
        onFilterRemovedByType: (filterType: string) => void
    }>{

    onDimensionChanged = (dimName: string, dimension:PickedDimension) => {
        this.props.onDimensionChanged(dimName, dimension);
    }

    onClearAxis = (axisName: string) => {
        this.props.onClearAxis(axisName);
    }

    render() {
        let visibility: string = this.props.hideControls ? " hide" : "";
        return (
            <div id={"LeftDock"} >
                <div className={visibility}>
                    <h4 className="Header">Day of week filter:</h4>
                    <DayOfWeekFilter onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters} onFilterRemoved={this.props.onFilterRemoved}></DayOfWeekFilter>
                </div>
                <div className={visibility}>
                    <h4 className="Header">Date filter:</h4>
                    <div className="date dropdowns">
                        <DateTagDropdown tagsetName={"Day within month"} onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters} onFilterReplaced={this.props.onFilterReplaced} onFilterRemoved={this.props.onFilterRemoved}/>
                        <DateTagDropdown tagsetName={"Month (number)"} onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters} onFilterReplaced={this.props.onFilterReplaced} onFilterRemoved={this.props.onFilterRemoved}/>
                        <DateTagDropdown tagsetName={"Year"} onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters} onFilterReplaced={this.props.onFilterReplaced} onFilterRemoved={this.props.onFilterRemoved}/>
                    </div>
                </div>
                <div className={visibility}>
                    <h4 className="Header">Time range filter:</h4>
                    <TimeFilter onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters} onFilterReplacedByType={this.props.onFilterReplacedByType} onFilterRemovedByType={this.props.onFilterRemovedByType}/>
                </div>
                <Dimensions className={visibility} activeFilters={this.props.activeFilters} onDimensionChanged={this.onDimensionChanged} onClearAxis={this.onClearAxis}/>
                <div className={visibility}>
                    <h4 className="Header">Tag filter:</h4>
                    <TagSearcher onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters}/>
                </div>
                <FilterList className ={visibility} activeFilters={this.props.activeFilters} onFilterRemoved={this.props.onFilterRemoved} />
	  		</div>
        );
    }
}