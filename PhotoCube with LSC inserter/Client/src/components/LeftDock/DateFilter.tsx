import React, { useEffect, useState } from 'react';
import { Filter } from '../Filter';
import { createFilter } from '../Middle/BottomDock/TagsetFilter';
import Fetcher from '../Middle/CubeBrowser/Fetcher';
import { Tag } from './Tag';
import Dropdown, { Option } from 'react-dropdown';
import '../../css/LeftDock/DateFilter.css';

/**
 * Component for browsing and adding date filters.
 * Currently used for adding tags from Year, Month (number) and Day within month tagsets.
 */
 export const DateTagDropdown = (props: {
     tagsetName: string, onFiltersChanged: (filter: Filter) => void, activeFilters: Filter[],
     onFilterReplaced: (oldFilter:Filter, newFilter: Filter) => void,
     onFilterRemoved : (filterId: number) => void }) => {

    const [options, setDropdownOptions] = useState<Tag[]>([]);
    const [previousFilter, updatePrevious] = useState<Filter | null>(null);
    const [selectedFilter, updateSelection] = useState<Filter | null>(null);
    const [displayed, updateDisplay] = useState<string>("");

    useEffect(() =>  {
        FetchTagsByTagsetName(); 
    }, []);

    async function FetchTagsByTagsetName () {
        const response = await Fetcher.FetchTagsByTagsetName(props.tagsetName);
        const tags: Tag[] = response.map((t: Tag) => {return {Id: t.Id, Name: t.Name }});
        //sort tags
        tags.sort((a,b) => parseInt(a.Name) - parseInt(b.Name));
        //format days and months
        const formattedTags = formatTags(tags);
        //set dropdown options
        setDropdownOptions(formattedTags);
    }

    const addFilter = (option: Tag) => {
        const filter: Filter = createFilter(option.Name, option.Id, "date");
        if (!props.activeFilters.some(af => af.Id === filter.Id)) {
            props.onFiltersChanged(filter);
            updatePrevious(filter);
            updateSelection(filter);
        }
    }

    const replaceFilter = (option: Tag) => {
        updatePrevious(selectedFilter);
        const newFilter: Filter = createFilter(option.Name, option.Id, "date");
        if (!props.activeFilters.some(af => af.Id === newFilter.Id)) {
            props.onFilterReplaced(selectedFilter!, newFilter);
            updateSelection(newFilter);
        }
    }

    const updateDropdown = (e: React.ChangeEvent<HTMLSelectElement>) => {
        updateDisplay(e.currentTarget.value);
        const selected: Tag = JSON.parse(e.currentTarget.value);
        previousFilter === null ? addFilter(selected) : replaceFilter(selected);
    }

    const onClear = () => {
        if (selectedFilter !== null) {
            props.onFilterRemoved(selectedFilter.Id);
            updatePrevious(null);
            updateSelection(null);
            updateDisplay("");
        }
    }

    return (
        <div className="date filter">
            <select className="Date Selector" value={displayed} onChange={(e) => updateDropdown(e)}>
                <option key={0} value={""}>{"Select "+ props.tagsetName.split(" ")[0]}</option>
                {options.map(option =>
                    <option key={option.Id} value={JSON.stringify(option)}>{option.Name}</option>)}
                </select>
            <button onClick={() => onClear()}>Clear</button>
        </div>
    )
}

//utility function 
export const formatTags = (months: Tag[]) => {
    return months.map((tag: Tag) => {
        if (tag.Name.length == 1) {
            return { Name: "0".concat(tag.Name), Id: tag.Id };
        } else {
            return tag;
        }
    })
}

