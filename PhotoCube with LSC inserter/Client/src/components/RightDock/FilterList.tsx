import React from 'react';
import { Filter } from '../Filter';
import '../../css/RightDock/FilterList.css';
import { AiOutlineCloseCircle } from 'react-icons/ai';

/**
 * List component used in the right dock to visualise all active filters.
 */
export const FilterList = 
    (props: {activeFilters: Filter[], className: string, onFilterRemoved: (filterId: number) => void}) => {
    return (
        <div className={props.className} id="filterlist-flexbox">
            <h4 className="Header">Active Filters</h4>
            <div id="filterlist-container">
                <ul className="filter list scrollable">
                    {props.activeFilters.map(filter => (filter.type !== "day of week" && filter.type !== "time" && filter.type !== "date") ?
                    <li key={filter.Id}>{filter.name}<button className="clear button" onClick={() => props.onFilterRemoved(filter.Id)}>
                        <AiOutlineCloseCircle id="clear-icon"/></button></li> : null
                    )}
                </ul>
            </div>
        </div>

    )
}