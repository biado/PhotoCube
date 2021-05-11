import React from 'react';
import { Filter } from '../Filter';
import '../../css/FilterList.css';
import { AiOutlineCloseCircle } from 'react-icons/ai';

export const FilterList = 
    (props: {activeFilters: Filter[], className: string, onFilterRemoved: (filterName: string) => void}) => {
    return (
        <div className={props.className}>
            <h4 className="Header">Active Filters</h4>
            <ul className="filter list scrollable">
                {props.activeFilters.map(filter => 
                    <li>{filter.name}<button className="clear button" onClick={e => props.onFilterRemoved(filter.name)}>
                        <AiOutlineCloseCircle id="clear-icon"/></button></li>
                )}
            </ul>
        </div>

    )
}