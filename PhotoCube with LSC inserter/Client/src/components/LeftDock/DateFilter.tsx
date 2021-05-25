import React, { useEffect, useState } from 'react';
import { Filter } from '../Filter';
import { createFilter } from '../Middle/BottomDock/TagsetFilter';
import Fetcher from '../Middle/CubeBrowser/Fetcher';
import { Tag } from './Tag';
import Dropdown, { Option } from 'react-dropdown';
import '../../css/LeftDock/TagFilter.css';

/**
 * Component for browsing and adding date filters.
 * Currently used for adding tags from Year, Month (number) and Day within month tagsets.
 */
 export const DateTagDropdown = (props: {
     tagsetName: string, onFiltersChanged: (filter: Filter) => void, activeFilters: Filter[],
     onFilterReplaced: (oldFilter:Filter, newFilter: Filter) => void,
     onFilterRemoved : (filterId: number) => void }) => {

    const [options, setDropdownOptions] = useState<Option[]>([]);
    const [previouslySelectedTag, updatePrevious] = useState<Tag | null>(null);
    const [selectedTag, updateSelection] = useState<Tag | null>(null);
    const [buttonDisabled, disableButton] = useState<boolean>(true);

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
        setDropdownOptions(formattedTags.map((t: Tag) => {return {value: t.Id.toString(), label: t.Name}}));
    }

    const addFilter = () => {
        if (selectedTag !== null) {
            const filter: Filter = createFilter(selectedTag!.Name, selectedTag!.Id, "date", "", "");
            if (!props.activeFilters.some(af => af.Id === filter.Id)) {
                props.onFiltersChanged(filter);
                disableButton(true);
            }
        }
    }

    const replaceFilter = () => {
        updatePrevious(selectedTag);
        const oldFilter: Filter = createFilter(previouslySelectedTag!.Name, previouslySelectedTag!.Id, "date", "", "");
        const newFilter: Filter = createFilter(selectedTag!.Name, selectedTag!.Id, "date", "", "");
        if (!props.activeFilters.some(af => af.Id === newFilter.Id)) {
            props.onFilterReplaced(oldFilter, newFilter);
            disableButton(true);
        }
    }

    const updateDropdown = (e: Option) => {
        updatePrevious(selectedTag);
        updateSelection({Id: parseInt(e.value), Name: e.label!.toString()});
        disableButton(props.activeFilters.some(af => af.Id === parseInt(e.value)));
    }

    const onClear = () => {
        if (selectedTag !== null) {
            props.onFilterRemoved(selectedTag.Id);
            updatePrevious(null);
            updateSelection(null);
        }
    }

    return (
        <div className="Filter">
            <button onClick={() => onClear()}>Clear</button>
            <Dropdown options={options}  placeholder={"Select "+ props.tagsetName} onChange={e => updateDropdown(e)}/>
            <button className="add button" disabled={buttonDisabled} onClick={() => (previouslySelectedTag === null) ? addFilter() : replaceFilter() }>Add filter</button>
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

