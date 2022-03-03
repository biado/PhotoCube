import React, { useEffect, useState } from 'react';
import { Filter } from '../Filter';
import { createFilter } from '../Middle/BottomDock/TagsetFilter';
import Fetcher from '../Middle/CubeBrowser/Fetcher';
import { Tag } from './Tag';
import '../../css/LeftDock/DateFilter.css';
import { set } from 'mobx';

/**
 * Component for browsing and adding date filters.
 * Currently used for adding tags from Year, Month (number) and Day within month tagsets.
 */

 export const DateTagDropdown = (props: {
     tagsetName: string, 
     onFiltersChanged: (filter: Filter) => void, 
     activeFilters: Filter[],
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
        //console.log(response);
        //const tags: Tag[] = response.map((t: Tag) => {return {id: t.id, name: t.name}});
        const tags: Tag[] = response.map((t: Tag) => {return {id: t.id, name: t.name, tagset: t.tagset}});

        //sort tags
        tags.sort((a,b) => a.name.localeCompare(b.name)); //sorting alphanumerical tags
        //tags.sort((a,b) => parseInt(a.name) - parseInt(b.name));

        //format days and months
        //const formattedTags = formatTags(filtertags);
        if(props.tagsetName=="month"){
            const formattedMonths = formatMonths(tags);
            setDropdownOptions(formattedMonths)
        } 
        if(props.tagsetName=="year"){
            //const filtertags = tags.filter((t:Tag) => t.name.length!=5) //remove decade
            const formattedYears = tags.filter((t:Tag) => t.name.charAt(t.name.length-1)!='s' && t.name!='year') //remove decades: 1900s, 1910s ...
            setDropdownOptions(formattedYears)
        }else{
            //set dropdown options
            setDropdownOptions(tags);
        }
    }

    const addFilter = (option: Tag) => {
        const filter: Filter = createFilter(option.name, option.id, "date");
        if (!props.activeFilters.some(af => af.id === filter.id)) {
            props.onFiltersChanged(filter);
            updatePrevious(filter);
            updateSelection(filter);
        }
    }

    const replaceFilter = (option: Tag) => {
        updatePrevious(selectedFilter);
        const newFilter: Filter = createFilter(option.name, option.id, "date");
        if (!props.activeFilters.some(af => af.id === newFilter.id)) {
            props.onFilterReplaced(selectedFilter!, newFilter);
            updateSelection(newFilter);
        }
    }

    const updateDropdown = (e: React.ChangeEvent<HTMLSelectElement>) => {
        if(e.currentTarget.value === ""){
            onClear();
        }
        else {
            updateDisplay(e.currentTarget.value);
            const selected: Tag = JSON.parse(e.currentTarget.value);
            previousFilter === null ? addFilter(selected) : replaceFilter(selected);
        }
    }

    const onClear = () => {
        if (selectedFilter !== null) {
            props.onFilterRemoved(selectedFilter.id);
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
                    <option key={option.id} value={JSON.stringify(option)}>{option.name}</option>)}
                </select>
            <button onClick={() => onClear()}>Clear</button>
        </div>
    )
}

//utility function , not in use for MTB
export const formatTags = (months: Tag[]) => {
    return months.map((tag: Tag) => {
        if (tag.name.length === 1) {
            //return { name: "0".concat(tag.name), id: tag.id};
            return { name: "0".concat(tag.name), id: tag.id, tagset:tag.tagset};
        } else {
            return tag;
        }
    })
}

export const formatMonths = (months: Tag[]) => {
    const monthsRef = ["January", "February", "March", "April", "May", "June",
    "July", "August", "September", "October", "November", "December", "missing"];
    months.sort(function(a, b) {
        return monthsRef.indexOf(a.name)- monthsRef.indexOf(b.name);
    })
    return months
}
