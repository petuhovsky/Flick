import React from 'react';
import cl from './ClearButton.module.css'

const ClearButton = ({children, ...props}) => {
    return (
        <button className={cl.clearButton} {...props}>
            {children}
        </button>
    );
};

export default ClearButton;