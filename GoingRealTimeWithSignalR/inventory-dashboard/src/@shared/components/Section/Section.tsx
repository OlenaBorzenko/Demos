import classNames from 'classnames';
import React, { HTMLAttributes } from 'react';
import { useStyles } from './Section.style';

interface Props extends HTMLAttributes<HTMLDivElement> {
  type: 'top' | 'main';
}

export const Section: React.FC<Props> = ({ className, type, children, ...divProps }) => {
  const s = useStyles();

  return (
    <div
      {...divProps}
      className={classNames(className, s[type])}
    >
      {children}
    </div>
  );
};

Section.displayName = 'Section';
