import React, { useEffect, useState } from 'react';

/**
 * functional slider component
 */
 export const ColorToggle : any = (props:{
    className: string, 
    onColorChange: (color:boolean) => void,
   }) => {

    const [color, updateColor] = useState<boolean>(false);
  
    useEffect(() =>  {
       //this.handleClick = this.handleClick.bind(this);  
}, []);
  
    //when opDirection and FilterValue changes a new filter is applied
    useEffect(() => {
      //console.log(color, '- Has changed')
      props.onColorChange(color)
    },[color])
  
  const handleClick = () =>{
    color ? updateColor(false) : updateColor(true) 
  }
      return (
        <div className={props.className}>
            <p> &nbsp;&nbsp;heatmap </p>
            <button onClick={handleClick}>        
                {color ? 'ON' : 'OFF'}
            </button>
        </div>
      );
     
  } 
