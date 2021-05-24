import React, { useEffect, useState } from "react";
import { Filter } from "../Filter";
import { createFilter } from "../Middle/BottomDock/TagsetFilter";
import '../../css/LeftDock/TagFilter.css';

/**
 * Component for adding a range query for Time tags.
 * Both the startTime and endTime must be filled for the 'Add filter' button to actually add the range filter.
 * To search for a distinct time, give a same value for both startTime and endTime text input field.
 */
export const TimeForm = (props: {
    activeFilters: Filter[],
    onFiltersChanged: (filter: Filter) => void
    onTimeFilterReplaced: (oldFilter:Filter, newFilter: Filter) => void,
    onFilterRemovedByType: (filterType: string) => void
}) => {
    const initialValues = {
        previousStartTime: "",
        previousEndTime: "",
        startTime: "",
        endTime: ""
    };
    const [values, setValues] = useState(initialValues);

    // Ref: https://dev.to/deboragaleano/how-to-handle-multiple-inputs-in-react-55el
    const handleInputChange = (e: { target: { name: any; value: any; }; }) => {
        const { name, value } = e.target;
        setValues({
            ...values,
            [name]: value,
        });
    }

    const addFilter = (e: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
        e.preventDefault();
        if (values.startTime !== "" && values.endTime !== "") {
            const filter: Filter = createFilter(values.startTime + "-" + values.endTime, 0, "time", values.startTime, values.endTime);
            if (!props.activeFilters.some(af => af.startTime === filter.startTime && af.endTime === filter.endTime)) {
                props.onFiltersChanged(filter);
            }
            values.previousStartTime = values.startTime;
            values.previousEndTime = values.endTime;
        }
    }

    const replaceFilter = (e: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
        e.preventDefault();
            if (values.startTime !== values.previousStartTime || values.endTime !== values.previousEndTime) {
                const oldFilter: Filter = createFilter(values.previousStartTime + "-" + values.previousEndTime, 0, "time", values.previousStartTime, values.previousEndTime);
                const newFilter: Filter = createFilter(values.startTime + "-" + values.endTime, 0, "time", values.startTime, values.endTime);
                if (props.activeFilters.some(af => af.type === "time")) {
                    props.onTimeFilterReplaced(oldFilter, newFilter);
                }
            }
    }

    const onClear = () => {
        if (values.startTime !== "" || values.endTime !== "") {
            setValues(initialValues);
            props.onFilterRemovedByType("time");
        }
    }

    return (
        <div>
            <button onClick={() => onClear()}>Clear</button>
        <form>
            <p className="Header">Start:</p>
            <input className="start time field" type="text" placeholder="00:00"
                    value={values.startTime}
                    onChange={handleInputChange}
                    name="startTime">
            </input>
            <p className="Header">End:</p>
            <input className="end time field" type="text" placeholder="23:59"
                    value={values.endTime}
                    onChange={handleInputChange}
                    name="endTime">
            </input>
            <div>
                <button className="add time range filter button" onClick={(e) => (values.previousStartTime === "" && values.previousEndTime === "") ? addFilter(e) : replaceFilter(e)}>Add filter</button>
            </div>
        </form>
        </div>
    )
}