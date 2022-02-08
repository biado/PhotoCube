import React from 'react';
import { Filter } from '../Filter';
import Fetcher from '../Middle/CubeBrowser/Fetcher';
import { Tag } from './Tag';
import { createFilter } from '../Middle/BottomDock/TagsetFilter';
import '../../css/LeftDock/DayOfWeekFilter.css';

/**
 * Component for applying tag filters from Day of week tagset.
 * The tag filters are applied when checking/unchecking the checkboxes.
 * IMPORTANT: Tag filters applied from this section will result in OR search.
 */
export default class DayOfWeekFilter extends React.Component<{
    onFiltersChanged: (filters: Filter) => void,
    activeFilters: Filter[],
    onFilterRemoved: (filterId: number) => void
}>{
    state = {
        daysOfWeek: [],
        //dayNames: ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"]
        dayNames: ["h", "s", "a", "f"] //["happy", "sad ", "anger ", "fear"]
    }

    render() {
        return (
            //<div className="emotion_code">
            <div className="dow filter">
                <ul>
                    {this.state.daysOfWeek.map((dow: Tag) => 
                        <li key={dow.id}>{this.renderDow(dow)}</li>
                    )}
                </ul>
            </div>
        );
    }

    componentDidMount() {
        this.renderDaysOfWeek();
    }

    /**
     * Fetches tags in Day of Week tagset from the server, and presents them with a checkbox.
     */
     private async renderDaysOfWeek() {
        //const response = await Fetcher.FetchTagsByTagsetName("Day of week (number)")
        const response = await Fetcher.FetchTagsByTagsetName("emotion_code")

        const DOW: Tag[] = response;
        console.log(DOW);
        DOW.sort((a,b) => parseInt(a.name) - parseInt(b.name));
        this.setState({daysOfWeek: DOW})
    }

    /**
     * Renders tags and checkboxes.
     */
    private renderDow(dowTag: Tag) {
        let inputElement = <input
            type="checkbox"
            name={dowTag.name}
            value={dowTag.id}
            onChange={e => this.onChange(e)} />;
        let result = <div className="dow checkbox">
                {inputElement}
                <p>{this.state.dayNames[parseInt(dowTag.name)]}</p>
                {/*<p>{this.state.dayNames[parseInt(dowTag.name)-1].substring(0,1)}</p>*/}
            </div>
        return result;
    }

    /**
     * If a checkbox is checked or unchecked, this method is called.
     * When checked: Adds a filter corresponding to the tag, and calls this.props.onFiltersChanged.
     * When unchecked: Removes the filter corresponding to the tag, and calls this.props.onFilterRemoved.
     */
     private onChange(e: React.ChangeEvent<HTMLInputElement>) {
        if (e.target.checked) {
            const filter: Filter = createFilter(e.target.name, parseInt(e.target.value), "day of week");
            //const filter: Filter = createFilter(e.target.name, parseInt(e.target.value), "emotion_code");
            //Add filter
            if (!this.props.activeFilters.some(af => af.name === e.target.name)) {
                this.props.onFiltersChanged(filter);
            }
        } else {
            //Remove filter
            const filterId = parseInt(e.target.value);
            if (this.props.activeFilters.some(af => af.id === filterId)) {
                this.props.onFilterRemoved(filterId);
            }
        }
    }
}
