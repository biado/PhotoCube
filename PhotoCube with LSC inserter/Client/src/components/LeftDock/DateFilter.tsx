import React, { useEffect, useState } from 'react';
import { Filter } from '../Filter';
import { createFilter } from '../Middle/BottomDock/TagsetFilter';
import Fetcher from '../Middle/CubeBrowser/Fetcher';
import { Tag } from './Tag';
import Dropdown, { Option } from 'react-dropdown';
import '../../css/LeftDock/TagFilter.css';

// export const DateTagDropdown = (props: {
//     tagsetName: string, onFiltersChanged: (filter: Filter) => void, activeFilters: Filter[],
//     onFilterReplaced: (oldFilter:Filter, newFilter: Filter) => void,
//     onFilterRemovedById : (filterId: number) => void }) => {

//     const [options, setDropdownOptions] = useState<Option[]>([]);
//     const [previouslySelectedTag, updatePrevious] = useState<Tag | null>(null);
//     const [selectedTag, updateSelectedTag] = useState<Tag | null>(null);
//     const [selectedTagName, updateSelectedTagName] = useState<string>("");
//     const [buttonDisabled, disableButton] = useState<boolean>(true);


//     useEffect(() =>  {
//         FetchTagsByTagsetName(); 
//     }, []);

//     async function FetchTagsByTagsetName () {
//         const response = await Fetcher.FetchTagsByTagsetName(props.tagsetName);
//         const tags = response.map((t: Tag) => {return {Id: t.Id, Name: t.Name }});
//         setDropdownOptions(tags.map((t: Tag) => {return {value: t.Id.toString(), label: t.Name}}));
//     }

//     const updateDropdown = (e: React.ChangeEvent<HTMLSelectElement>) => {
//         updatePrevious(selectedTag);
//         const filter: Filter = JSON.parse(e.currentTarget.value);
//         updateSelectedTag({Id: filter.Id, Name: filter.name});
//         updateSelectedTagName(filter.name);
//         disableButton(props.activeFilters.some(af => af.Id === filter.Id));
//     }

//     const addFilter = () => {
//         if (selectedTag !== null) {
//             const filter: Filter = createFilter(selectedTag!.Name, selectedTag!.Id, "tag", "", "");
//             if (!props.activeFilters.some(af => af.Id === filter.Id)) {
//                 props.onFiltersChanged(filter);
//                 disableButton(true);
//             }
//         }
//     }

//     const replaceFilter = () => {
//         updatePrevious(selectedTag);
//         const oldFilter: Filter = createFilter(previouslySelectedTag!.Name, previouslySelectedTag!.Id, "tag", "", "");
//         const newFilter: Filter = createFilter(selectedTag!.Name, selectedTag!.Id, "tag", "", "");
//         if (!props.activeFilters.some(af => af.Id === newFilter.Id)) {
//             props.onFilterReplaced(oldFilter, newFilter);
//             disableButton(true);
//         }
//     }

//     const onClear = () => {
//         if (selectedTag !== null) {
//             props.onFilterRemovedById(selectedTag.Id);
//             updatePrevious(null);
//             updateSelectedTag(null);
//             updateSelectedTagName("");
//         }
//     }
//     return (
//         <div className="Filter">
//             <button onClick={() => onClear()}>Clear</button>
//             <select className="Filter Selector" value={selectedTagName} onChange={(e) => updateDropdown(e)}>
//                 <option key={0} value={"true"}>{"Select" + props.tagsetName}</option>
//                 {options.map(af => 
//                     <option key={af.value} value={JSON.stringify(af)}>{af.label}</option>)}
//             </select>
//             <button className="add button" disabled={buttonDisabled} onClick={() => (previouslySelectedTag === null) ? addFilter() : replaceFilter() }>Add filter</button>
//         </div>
//     )
// }


/**
 * Component for browsing and adding date filters.
 * Currently used for adding tags from Year, Month (number) and Day within month tagsets.
 */
 export const DateTagDropdown = (props: {
     tagsetName: string, onFiltersChanged: (filter: Filter) => void, activeFilters: Filter[],
     onFilterReplaced: (oldFilter:Filter, newFilter: Filter) => void,
     onFilterRemovedById : (filterId: number) => void }) => {

    const [options, setDropdownOptions] = useState<Option[]>([]);
    const [previouslySelectedTag, updatePrevious] = useState<Tag | null>(null);
    const [selectedTag, updateSelection] = useState<Tag | null>(null);
    const [buttonDisabled, disableButton] = useState<boolean>(true);

    useEffect(() =>  {
        FetchTagsByTagsetName(); 
    }, []);

    async function FetchTagsByTagsetName () {
        const response = await Fetcher.FetchTagsByTagsetName(props.tagsetName);
        const tags = response.map((t: Tag) => {return {Id: t.Id, Name: t.Name }});
        setDropdownOptions(tags.map((t: Tag) => {return {value: t.Id.toString(), label: t.Name}}));
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
            props.onFilterRemovedById(selectedTag.Id);
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