#include <stdlib.h>
#import <UIKit/UIKit.h>

extern void _clearNotificationBadge () {
    [UIApplication sharedApplication].applicationIconBadgeNumber = 0;
}
