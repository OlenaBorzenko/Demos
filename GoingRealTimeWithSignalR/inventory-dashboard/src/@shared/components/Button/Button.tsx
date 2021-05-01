import classNames from 'classnames';
import React from 'react';

import { useStyles } from './Button.style';

export interface ButtonProps {
  className?: string;
  disabled?: boolean;
  hidden?: boolean;
  onClick?: (e?: React.MouseEvent) => void;
  theme?: 'blue' | 'grey' | 'link' | 'chip';
  size?: 'small' | 'medium' | 'large' | 'fitToParent';
  type?: 'button' | 'submit';
  title?: string;
}

/** A button with a configurable background color. */
export const Button: React.FC<ButtonProps> =
  ({ className, onClick, disabled, hidden, theme, size, type, title, children }) => {
    const s = useStyles();

    const buttonClassName = classNames(
      s.button,
      theme,
      size,
      className,
    );

    return (
      <button
        className={buttonClassName}
        onClick={onClick}
        disabled={disabled}
        hidden={hidden}
        type={type}
      >
        {title || children}
      </button>
    );
  };

Button.defaultProps = {
  theme: 'blue',
  disabled: false,
  hidden: false,
  size: 'medium',
  type: 'button',
};

Button.displayName = 'Button';
