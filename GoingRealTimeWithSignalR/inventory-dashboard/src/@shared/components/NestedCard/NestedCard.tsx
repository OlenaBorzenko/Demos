import React, { useEffect, useState } from 'react';
import { Card, CardContent } from '@material-ui/core';
import { observer } from 'mobx-react';
import clsx from 'clsx';
import { NestedCardConfig } from '@shared/components/NestedCard/NestedCardConfig';
import { useStyles } from '@shared/components/NestedCard/NestedCard.style';

interface Props {
  cardConfig: NestedCardConfig[];
  item: any;
}

export const NestedCard: React.FC<Props> = observer(props => {
  const { item, cardConfig } = props;
  const s = useStyles();
  const [updated, setUpdated] = useState(false);

  useEffect(() => {
    setUpdated(true);

    setTimeout(function () {
      setUpdated(false);
    }, 3000);

  }, [item, item.quantity]);

  return (
    <Card ref={React.createRef()} className={clsx(s.nestedCard, {
      [s.updated]: updated,
    })}>
      <CardContent>
        {cardConfig.map((prop, x) => (
          <div key={`${name}-${x}`}>
            {
              typeof prop.content === 'function'
                ? prop.content(item)
                : prop.content
            }
          </div>
        ))}
      </CardContent>
    </Card>
  );
});
